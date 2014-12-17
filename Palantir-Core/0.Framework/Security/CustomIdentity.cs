namespace Ix.Palantir.Security
{
    using System;
    using System.Runtime.Serialization;
    using System.Security.Principal;
    using Ix.Framework.ObjectFactory;
    using Ix.Palantir.Security.API;

    [Serializable]
    public class CustomIdentity : GenericIdentity, ISerializable
    {
        private User user;

        public CustomIdentity(string name) : this(name, "Forms")
        {
        }

        public CustomIdentity(string name, string type) : base(name, type)
        {
        }

        public int Id
        {
            get { return this.User.Id; }
        }

        public override string Name
        {
            get { return this.User.FullName; }
        }

        public string TimeZoneName
        {
            get { return this.User.TimeZoneName; }
        }

        public User User
        {
            get
            {
                if (this.user == null)
                {
                    this.user = Factory.GetInstance<IUserRepository>().GetUserByEmail(base.Name);
                }

                return this.user;
            }
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (context.State == StreamingContextStates.CrossAppDomain)
            {
                GenericIdentity genericIdentity = new GenericIdentity(this.Name, this.AuthenticationType);
                info.SetType(genericIdentity.GetType());

                System.Reflection.MemberInfo[] serializableMembers;
                object[] serializableValues;

                serializableMembers = FormatterServices.GetSerializableMembers(genericIdentity.GetType());
                serializableValues = FormatterServices.GetObjectData(genericIdentity, serializableMembers);

                for (int i = 0; i < serializableMembers.Length; i++)
                {
                    info.AddValue(serializableMembers[i].Name, serializableValues[i]);
                }
            }
            else
            {
                throw new InvalidOperationException("Serialization not supported");
            }
        }
    }
}