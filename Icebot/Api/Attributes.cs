using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Icebot.Api
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class PluginMetadataAttribute : Attribute
    {
        public PluginMetadataAttribute()
        {
        }

        public string Title { get; set; }
        public string Author { get; set; }
        public string Description { get; set; }
    }

    [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    internal class PluginInstanceTypeAttribute : Attribute
    {
        public PluginInstanceTypeAttribute()
        {
        }

        internal InstanceType InstanceType { get; internal set; }
    }

    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class ChannelPluginAttribute : PluginInstanceTypeAttribute
    {
        public ChannelPluginAttribute()
        {
            this.InstanceType = InstanceType.OnePerChannel;
        }
    }

    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class ServerPluginAttribute : PluginInstanceTypeAttribute
    {
        public ServerPluginAttribute()
        {
            this.InstanceType = InstanceType.OnePerServer;
        }
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public sealed class PluginCommandAttribute : Attribute
    {
        /*
        // TODO: Automatic plugin command name detection from the method's name.
        public PluginCommandAttribute()
        {
        }
         */
        public PluginCommandAttribute(string commandName)
        {
            this.Name = commandName;
        }
        public PluginCommandAttribute(string commandName, CommandType type)
        {
            this.Name = commandName;
            this.Type = type;
        }
        public PluginCommandAttribute(string commandName, string description)
        {
            this.Name = commandName;
            this.ShortDescription = description;
        }
        public PluginCommandAttribute(string commandName, string description, CommandType type)
        {
            this.Name = commandName;
            this.ShortDescription = description;
            this.Type = type;
        }
        public PluginCommandAttribute(string commandName, string description, string longdescription)
        {
            this.Name = commandName;
            this.ShortDescription = description;
            this.LongDescription = longdescription;
        }
        public PluginCommandAttribute(string commandName, string description, string longdescription, CommandType type)
        {
            this.Name = commandName;
            this.ShortDescription = description;
            this.LongDescription = longdescription;
            this.Type = type;
        }

        private string _name;
        private string _shortDescription;
        private string _longDescription;
        private CommandType _type = CommandType.Channel;

        public string Name
        {
            get { return _name; }
            set
            {
                if (!Limits.IsValidCommandName(value))
                    throw new Exception("The command name is not valid.");
            }
        }
        public string ShortDescription
        {
            get { return _shortDescription; }
            set
            {
                if (value.Length > Limits.MaximumShortDescriptionLength)
                    throw new Exception("Short description must not be longer than 384 characters.");
                if (value.Length > _longDescription.Length && Limits.LongMustBeLongerThanShortDescription)
                    throw new Exception("Short description length must not exceed long description length");
                _shortDescription = value;
            }
        }
        public string LongDescription
        {
            get { return _longDescription; }
            set
            {
                if (value.Length < _shortDescription.Length && Limits.LongMustBeLongerThanShortDescription)
                    throw new Exception("Long description must not be shorter than short description");
                _longDescription = value;
            }
        }
        public CommandType Type {
            get { return _type; }
            set { _type = value; }
        }
    }

    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple=false)]
    public sealed class PluginCommandParameterAttribute : Attribute
    {
        public PluginCommandParameterAttribute()
        {
        }
        public PluginCommandParameterAttribute(string description)
        {
            this.Description = description;
        }
        public string Description { get; set; }
    }
}
