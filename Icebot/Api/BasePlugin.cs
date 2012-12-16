using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;

namespace Icebot.Api
{
    public class BasePlugin : IDisposable
    {
        #region Logging functions
        private ILog _log;
        protected void Debug(string text)
        {
            _log.Debug(text);
        }
        protected void Debug(string text, params object[] arg)
        {
            _log.DebugFormat(text, arg);
        }
        protected void Info(string text)
        {
            _log.Info(text);
        }
        protected void Info(string text, params object[] arg)
        {
            _log.InfoFormat(text, arg);
        }
        protected void Warn(string text)
        {
            _log.Warn(text);
        }
        protected void Warn(string text, params object[] arg)
        {
            _log.WarnFormat(text, arg);
        }
        protected void Error(string text)
        {
            _log.Error(text);
        }
        protected void Error(string text, params object[] arg)
        {
            _log.ErrorFormat(text, arg);
        }
        #endregion

        public BasePlugin()
        {
            _log = LogManager.GetLogger(this.GetType());
        }

        public virtual void AssignServer()
        {
            throw new NotSupportedException();
        }

        public virtual void AssignChannel()
        {
            throw new NotSupportedException();
        }

        public virtual void UnassignServer()
        {
            throw new NotSupportedException();
        }

        public virtual void UnassignChannel()
        {
            throw new NotSupportedException();
        }
    }
}
