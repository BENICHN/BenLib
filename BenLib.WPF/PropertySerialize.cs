using System;

namespace BenLib.WPF
{
    public interface ISerializedProperty
    {
        string Name { get; }
        Action<object, object> Setter { get; }
    }
    public interface ISerializedProperty<TOwner> : ISerializedProperty { new Action<TOwner, object> Setter { get; } }
    public interface ISerializedProperty<TOwner, TProperty> : ISerializedProperty<TOwner> { new Action<TOwner, TProperty> Setter { get; } }

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

    public class SerializedProperty<TOwner> : ISerializedProperty<TOwner>
    {
        public SerializedProperty(string name, Action<TOwner, object> setter)
        {
            Name = name;
            Setter = setter;
        }

        public string Name { get; }
        public Action<TOwner, object> Setter { get; }
        Action<object, object> ISerializedProperty.Setter => (o, p) => Setter((TOwner)o, p);
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
        Action<TOwner, object> ISerializedProperty<TOwner>.Setter => (o, p) => Setter(o, (TProperty)p);
        Action<object, object> ISerializedProperty.Setter => (o, p) => Setter((TOwner)o, (TProperty)p);
    }
}
