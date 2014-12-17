namespace Ix.Palantir.Scheduler.Runner
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.Reflection;
    using System.Threading;
    using System.Xml;
    using Exceptions;
    using Framework.ObjectFactory;

    public class ReflectionBasedAction : AbstractAction
    {
        public ReflectionBasedAction(string name)
        {
            this.mListenerQualifiedName = null;
            this.mListenerType = null;
            this.InternalName = name;
        }

        public override ActionResult Execute(ActionContext context)
        {
            IActionListener listener1 = null;
            ActionResult result1 = new ActionResult();
            lock (this.SynchObject)
            {
                Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;

                if (this.mListenerType == null)
                {
                    throw new InvalidOperationException("Action listener is not set for this action.");
                }

                listener1 = (IActionListener)Factory.GetInstance(this.mListenerType);
            }

            try
            {
                result1.SetResult(listener1.FireAction(context));
            }
            catch (Exception exception1)
            {
                if (ExceptionHelper.IsFatalException(exception1))
                {
                    throw;
                }

                result1.SetActionState(ActionState.Exception);
                result1.SetException(exception1);
            }

            return result1;
        }

        [SuppressMessage("Microsoft.Reliability", "CA2001:AvoidCallingProblematicMethods", MessageId = "System.Reflection.Assembly.LoadFile", Justification = "Reviewed. Suppression is OK here.")]
        internal override void ParseXML(string[] assemblyPaths, XmlNode node)
        {
            XmlElement element1 = (XmlElement)node.SelectSingleNode("./ActionListener");
            string text1 = element1.GetAttribute("class");
            string text2 = element1.GetAttribute("assembly");
            Assembly assembly1 = null;
            try
            {
                assembly1 = AppDomain.CurrentDomain.Load(text2);
            }
            catch (Exception ex)
            {
                if (ExceptionHelper.IsFatalException(ex))
                {
                    throw;
                }
            }

            foreach (string text3 in assemblyPaths)
            {
                try
                {
                    assembly1 = Assembly.LoadFile(text3 + @"\" + text2 + ".dll");
                    if (assembly1 == null)
                    {
                        goto Label_006D;
                    }
                }
                catch (Exception ex)
                {
                    if (ExceptionHelper.IsFatalException(ex))
                    {
                        throw;
                    }
                }

                break;
                Label_006D:
                continue;
            }

            if (assembly1 == null)
            {
                throw new ArgumentException("Cannot find assembly: " + text2);
            }

            assembly1.GetType(text1);
            this.SetActionListener(text1, assembly1);
        }

        public override void SetActionListener(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            lock (this.SynchObject)
            {
                this.mListenerType = type;
                this.mListenerQualifiedName = type.AssemblyQualifiedName;
            }
        }

        public override void SetActionListener(string className, Assembly assembly)
        {
            Type type1 = assembly.GetType(className);
            if (type1 == null)
            {
                throw new ArgumentException("Cannot find " + className + " in assembly " + assembly.FullName + ".");
            }

            this.SetActionListener(type1);
        }

        [SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields", Justification = "Reviewed. Suppression is OK here.")]
        private string mListenerQualifiedName;
        private Type mListenerType;
    }
}