using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Icebot.Api;

namespace Icebot.Plugins
{
    /*
     * First: don't be scared about the big amount of lines used for this
     * simple plugin. It's just because of the length of the commentaries
     * on the parts of the plugin.
     * 
     * To make a plugin, you need to create a class which is getting its
     * base from Icebot.Api.BasePlugin, then describe it by setting a
     * few attributes and finally make the commands for it.
     */

    /*
     * 1st step:
     * "Explain" how the plugin is to be loaded. You have the choice
     * to make it a singleton-instance plugin, so the plugin can work
     * across multiple channels or to make it being loaded in channel-
     * seperated instances. Note that you can NOT mark a plugin as both
     * Channel- and ServerPlugin at the same time.
     */

    // Indicates that the bot has to make a single instance for every
    // channel the plugin is requested to work in.
    [ChannelInstancePlugin]

    // Indicates that the bot has to share a single instance with every
    // channel of a server the plugin is requested to work in.
    //[ServerPlugin]

    /*
     * 2nd step:
     * Give some information about the plugin for the help page to make
     * clear in text form what the plugin does, what it even is and who
     * wrote it.
     * 
     * For this you can use the PluginMetadata attribute. It is not a
     * required attribute, but it's good to have a well-documented plugin!
     */
    [PluginTitle("Greetings")] // Give the plugin a title.
    [PluginDescription("Makes the bot greet people automatically on join")] // Describe the plugin a bit here.
    [PluginAuthor("Carl Kittelberger")] // who actually wrote it? Can be just "[nickname/real name]" or "[nickname/realname] <[email here]>".

    /*
     * Now off to the actual code. Create a derivated class from
     *    Icebot.Api.BasePlugin.
     * The name of the class does not even matter.
     */
    public class TestPlugin : BasePlugin
    {
        /*
         * A channel plugin needs to override AssignChannel() and use its own code
         * to handle assignments to a channel of course.
         * 
         * By that I just mean: Make it do something when activated in a channel.
         * 
         * You can also make it do something when assigned to a server, not only
         * to a channel. In this case, override AssignServer() with your very own
         * code.
         */
        public override void AssignChannel()
        {

        }

        /*
         * Now let's define a command.
         * 
         * This command is public (available on channels) and defines !test (or ?test,
         * depending on whatever your command prefix on the channel is). It also has
         * no arguments.
         * 
         * The intended syntax for making a method for this command is:
         * 
         *     public void [somenamehere]( [arguments] )
         *     
         * The funny trick behind [arguments] is, that it is epicly dynamic and you can
         * structure as you want. Just a few things to consider:
         *     - If you need the IRC user who sent the message, define exactly 1 argument of
         *       type IrcSource. Usually you name this argument "user."
         *     - If you need the IRC channel on which the message has been sent, define
         *       exactly 1 argument of type IrcChannel. Usually you name this argument
         *       "channel."
         *     - If you need the IRC server on which the channel is hosted, define exactly
         *       1 argument of type IrcServer. Usually you name this argument "server."
         * The rest of the parameters are up to you how to define them. You will mostly need
         * them to take the arguments given by the one who called the functions. You can also
         * just define no custom arguments. Would make it a parameter-less command.
         */
        [PluginCommand("test")]
        public void Public_Test(IrcChannel channel, IrcUser user)
        {
            user.Notice("Test in " + channel.Name + " succeeded!");
        }

        /*
         * This command is public, defines !testarg and can receive an "endless" amount
         * of items, as no limit is set to the parameter count ("params"). Of course you
         * can also leave the params away and instead for example define three string-type
         * arguments.
         * 
         * Note that you can NOT use arrays as type for non-params arguments.
         */
        [PluginCommand("testarg")]
        public void Public_TestArguments(

            // The sources given by the bot itself
            IrcChannel channel,
            IrcUser source,

            // The actual arguments of the command call are stored here
            [PluginCommandParameterDescription("The arguments")] // will later show up in the help as a description for the "arguments" parameter
            params string[] arguments
        )
        {
            source.Notice("Test in " + channel.Name + " succeeded, I got following arguments:");
            foreach (string argument in arguments)
                source.Notice("=> " + argument);
        }

        /*
         * This command is private.
         */
        [PluginCommand("testadd", CommandType.Private)]
        [PluginCommandDescription(
            "Calculates the addition of two numbers.",
            "Calculates the result of A + B and outputs it via a notice. This is also called the 'addition' of two numbers A and B."
        )]
        public void Private_TestAddition(
            IrcUser source,

            [PluginCommandParameterDescription("First number of addition")]
            int A,

            [PluginCommandParameterDescription("Second number of addition")]
            int B
        )
        {
            source.Notice(A.ToString() + " + " + B.ToString() + " = " + (A + B).ToString());
        }
    }
}
