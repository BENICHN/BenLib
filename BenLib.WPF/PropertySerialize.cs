using System;

namespace BenLib.WPF
{
    public interface ISerializedProperty
    {
        string Name { get; }
        Action<object, object> Setter { get; }
    }

    public interface ISerializedProperty<TOwner, TProperty> : ISerializedProperty { new Action<TOwner, TProperty> Setter { get; } }

    public class SerializedProperty : ISerializedProperty
    {
        public SerializedProperty(string name, Action<object, object> setter)
        {
            Name = name;
            Setter = setter;
        }

        public string Name { get; }
        public Action<object, object> Setter { get; }
    }

    public class SerializedProperty<TOwner, TProperty> : ISerializedProperty<TOwner, TProperty>
    {
        public SerializedProperty(string name, Action<TOwner, TProperty> setter)
        {
            Name = name;
            Setter = setter;
        }

        public string Name { get; }
        public Action<TOwner, TProperty> Setter { get; }
        Action<object, object> ISerializedProperty.Setter => (o, p) => Setter((TOwner)o, (TProperty)p);
    }
}
