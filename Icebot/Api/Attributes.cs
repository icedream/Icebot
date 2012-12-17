using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Icebot.Api
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class PluginTitleAttribute : Attribute
    {
        public PluginTitleAttribute(string title)
        {
            this.Title = title;
        }

        public string Title { get; set; }
    }
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class PluginAuthorAttribute : Attribute
    {
        public PluginAuthorAttribute(string author)
        {
            this.Author = author;
        }

        public string Author { get; set; }
    }
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class PluginDescriptionAttribute : Attribute
    {
        public PluginDescriptionAttribute(string description)
        {
            this.Description = description;
        }

        public string Description { get; set; }
    }

    [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    internal class PluginInstanceTypeAttribute : Attribute
    {
        internal PluginInstanceTypeAttribute()
        {
        }
        public PluginInstanceTypeAttribute(InstanceType type)
        {
            this.InstanceType = type;
        }

        internal InstanceType InstanceType { get; internal set; }
    }

    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class ChannelInstancePluginAttribute : PluginInstanceTypeAttribute
    {
        public ChannelInstancePluginAttribute()
        {
            this.InstanceType = InstanceType.OnePerChannel;
        }
    }

    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class ServerInstancePluginAttribute : PluginInstanceTypeAttribute
    {
        public ServerInstancePluginAttribute()
        {
            this.InstanceType = InstanceType.OnePerServer;
        }
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public sealed class PluginCommandAttribute : Attribute
    {
        public PluginCommandAttribute(string commandName)
        {
            this.Name = commandName;
        }
        public PluginCommandAttribute(string commandName, CommandType type)
        {
            this.Name = commandName;
            this.Type = type;
        }

        private string _name;
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
        public CommandType Type {
            get { return _type; }
            set { _type = value; }
        }
    }
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public sealed class PluginCommandDescriptionAttribute : Attribute
    {
        public PluginCommandDescriptionAttribute(string description)
        {
            ShortDescription = LongDescription = description;
        }
        public PluginCommandDescriptionAttribute(string shortDescription, string longDescription)
        {
            this.ShortDescription = shortDescription;
            this.LongDescription = longDescription;
        }

        private string _shortDescription;
        private string _longDescription;

        public string ShortDescription
        {
            get { return _shortDescription; }
            set
            {
                if (value.Length > Limits.MaximumShortDescriptionLength)
                    throw new Exception("Short description must not be longer than 384 characters.");
                if (LongDescription != null && value.Length > _longDescription.Length && Limits.LongMustBeLongerThanShortDescription)
                    throw new Exception("Short description length must not exceed long description length");
                _shortDescription = value;
            }
        }
        public string LongDescription
        {
            get { return _longDescription; }
            set
            {
                if (ShortDescription != null && value.Length < _shortDescription.Length && Limits.LongMustBeLongerThanShortDescription)
                    throw new Exception("Long description must not be shorter than short description");
                _longDescription = value;
            }
        }
    }

    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple=false)]
    public sealed class PluginCommandParameterDescriptionAttribute : Attribute
    {
        public PluginCommandParameterDescriptionAttribute(string description)
        {
            this.Description = description;
        }
        public string Description { get; set; }
    }
}
