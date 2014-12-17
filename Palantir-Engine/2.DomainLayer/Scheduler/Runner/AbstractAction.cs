namespace Ix.Palantir.Scheduler.Runner
{
    using System;
    using System.Reflection;
    using System.Xml;

    public abstract class AbstractAction
    {
        protected AbstractAction()
        {
            this.InternalName = null;
            this.SynchObject = new object();
        }

        public abstract ActionResult Execute(ActionContext context);
        internal abstract void ParseXML(string[] assemblyPaths, XmlNode node);
        public abstract void SetActionListener(Type type);
        public abstract void SetActionListener(string className, Assembly assembly);

        public virtual string Name
        {
            get
            {
                return this.InternalName;
            }
        }

        protected internal string InternalName { get; set; }
        protected object SynchObject { get; set; }
    }
}