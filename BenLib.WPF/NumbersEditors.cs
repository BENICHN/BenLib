using BenLib.Framework;
using BenLib.Standard;
using System;
using System.Windows;
using static System.Math;

namespace BenLib.WPF
{
    public class DoubleEditor : SwitchableTextBox
    {
        private double m_baseValue;

        public double Value { get => (double)GetValue(ValueProperty); set => SetValue(ValueProperty, value); }
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(double), typeof(DoubleEditor));

        public bool IsUnsigned { get => (bool)GetValue(IsUnsignedProperty); set => SetValue(IsUnsignedProperty, value); }
        public static readonly DependencyProperty IsUnsignedProperty = DependencyProperty.Register("IsUnsigned", typeof(bool), typeof(DoubleEditor));

        public double IncrementFactor { get => (double)GetValue(IncrementFactorProperty); set => SetValue(IncrementFactorProperty, value); }
        public static readonly DependencyProperty IncrementFactorProperty = DependencyProperty.Register("IncrementFactor", typeof(double), typeof(DoubleEditor), new PropertyMetadata(0.1));

        public event PropertyChangedExtendedEventHandler<double> ValueChanged;

        static DoubleEditor()
        {
            DragProperty.OverrideMetadata(typeof(DoubleEditor), new PropertyMetadata(true));
            TextProperty.OverrideMetadata(typeof(DoubleEditor), new PropertyMetadata("0"));
            TextValidationProperty.OverrideMetadata(typeof(DoubleEditor), new PropertyMetadata(new Predicate<string>(s => s.IsDouble().ShowException())));
        }
        public DoubleEditor()
        {
            AllowedStrings = new[] { "-∞", "∞", "NaN" };
            ContentType = ContentType.Decimal;
            Desactivated += (sender, e) => Value = Text.ToDouble() ?? Value;
        }

        protected override void OnIncrement(double value)
        {
            if (DragValue == value) m_baseValue = Value;
            double v = m_baseValue + DragValue * IncrementFactor;
            Value = Round(IsUnsigned ? Max(0, v) : v, 4);
        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.Property == ValueProperty)
            {
                Text = ((double)e.NewValue).ToString();
                ValueChanged?.Invoke(this, new PropertyChangedExtendedEventArgs<double>("Value", (double)e.OldValue, (double)e.NewValue));
            }
            else if (e.Property == IsUnsignedProperty)
            {
                if ((bool)e.NewValue)
                {
                    ContentType = ContentType.UnsignedDecimal;
                    TextValidation = s => s.IsDouble(out double d).ShowException() && d >= 0;
                }
                else
                {
                    ContentType = ContentType.Decimal;
                    TextValidation = s => s.IsDouble().ShowException();
                }
            }
            base.OnPropertyChanged(e);
        }
    }

    public class FloatEditor : SwitchableTextBox
    {
        private float m_baseValue;

        public float Value { get => (float)GetValue(ValueProperty); set => SetValue(ValueProperty, value); }
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(float), typeof(FloatEditor));

        public bool IsUnsigned { get => (bool)GetValue(IsUnsignedProperty); set => SetValue(IsUnsignedProperty, value); }
        public static readonly DependencyProperty IsUnsignedProperty = DependencyProperty.Register("IsUnsigned", typeof(bool), typeof(FloatEditor));

        public double IncrementFactor { get => (double)GetValue(IncrementFactorProperty); set => SetValue(IncrementFactorProperty, value); }
        public static readonly DependencyProperty IncrementFactorProperty = DependencyProperty.Register("IncrementFactor", typeof(double), typeof(FloatEditor), new PropertyMetadata(0.1));

        public event PropertyChangedExtendedEventHandler<float> ValueChanged;

        static FloatEditor()
        {
            DragProperty.OverrideMetadata(typeof(FloatEditor), new PropertyMetadata(true));
            TextProperty.OverrideMetadata(typeof(FloatEditor), new PropertyMetadata("0"));
            TextValidationProperty.OverrideMetadata(typeof(FloatEditor), new PropertyMetadata(new Predicate<string>(s => s.IsFloat().ShowException())));
        }
        public FloatEditor()
        {
            ContentType = ContentType.Decimal;
            Desactivated += (sender, e) => Value = Text.ToFloat() ?? Value;
        }

        protected override void OnIncrement(double value)
        {
            if (DragValue == value) m_baseValue = Value;
            float v = m_baseValue + (float)(DragValue * IncrementFactor);
            Value = IsUnsigned ? Max(0, v) : v;
        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.Property == ValueProperty)
            {
                Text = ((float)e.NewValue).ToString();
                ValueChanged?.Invoke(this, new PropertyChangedExtendedEventArgs<float>("Value", (float)e.OldValue, (float)e.NewValue));
            }
            else if (e.Property == IsUnsignedProperty)
            {
                if ((bool)e.NewValue)
                {
                    ContentType = ContentType.UnsignedDecimal;
                    TextValidation = s => s.IsFloat(out float f).ShowException() && f >= 0;
                }
                else
                {
                    ContentType = ContentType.Decimal;
                    TextValidation = s => s.IsFloat().ShowException();
                }
            }
            base.OnPropertyChanged(e);
        }
    }

    public class DecimalEditor : SwitchableTextBox
    {
        private decimal m_baseValue;

        public decimal Value { get => (decimal)GetValue(ValueProperty); set => SetValue(ValueProperty, value); }
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(decimal), typeof(DecimalEditor));

        public bool IsUnsigned { get => (bool)GetValue(IsUnsignedProperty); set => SetValue(IsUnsignedProperty, value); }
        public static readonly DependencyProperty IsUnsignedProperty = DependencyProperty.Register("IsUnsigned", typeof(bool), typeof(DecimalEditor));

        public double IncrementFactor { get => (double)GetValue(IncrementFactorProperty); set => SetValue(IncrementFactorProperty, value); }
        public static readonly DependencyProperty IncrementFactorProperty = DependencyProperty.Register("IncrementFactor", typeof(double), typeof(DecimalEditor), new PropertyMetadata(0.1));

        public event PropertyChangedExtendedEventHandler<decimal> ValueChanged;

        static DecimalEditor()
        {
            DragProperty.OverrideMetadata(typeof(DecimalEditor), new PropertyMetadata(true));
            TextProperty.OverrideMetadata(typeof(DecimalEditor), new PropertyMetadata("0"));
            TextValidationProperty.OverrideMetadata(typeof(DecimalEditor), new PropertyMetadata(new Predicate<string>(s => s.IsDecimal().ShowException())));
        }
        public DecimalEditor()
        {
            ContentType = ContentType.Decimal;
            Desactivated += (sender, e) => Value = Text.ToDecimal() ?? Value;
        }

        protected override void OnIncrement(double value)
        {
            if (DragValue == value) m_baseValue = Value;
            decimal v = m_baseValue + (decimal)(DragValue * IncrementFactor);
            Value = Round(IsUnsigned ? Max(0, v) : v, 4);
        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.Property == ValueProperty)
            {
                Text = ((decimal)e.NewValue).ToString();
                ValueChanged?.Invoke(this, new PropertyChangedExtendedEventArgs<decimal>("Value", (decimal)e.OldValue, (decimal)e.NewValue));
            }
            else if (e.Property == IsUnsignedProperty)
            {
                if ((bool)e.NewValue)
                {
                    ContentType = ContentType.UnsignedDecimal;
                    TextValidation = s => s.IsDecimal(out decimal m).ShowException() && m >= 0;
                }
                else
                {
                    ContentType = ContentType.Decimal;
                    TextValidation = s => s.IsDecimal().ShowException();
                }
            }
            base.OnPropertyChanged(e);
        }
    }

    public class IntEditor : SwitchableTextBox
    {
        private int m_baseValue;

        public int Value { get => (int)GetValue(ValueProperty); set => SetValue(ValueProperty, value); }
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(int), typeof(IntEditor));

        public bool IsUnsigned { get => (bool)GetValue(IsUnsignedProperty); set => SetValue(IsUnsignedProperty, value); }
        public static readonly DependencyProperty IsUnsignedProperty = DependencyProperty.Register("IsUnsigned", typeof(bool), typeof(IntEditor));

        public double IncrementFactor { get => (double)GetValue(IncrementFactorProperty); set => SetValue(IncrementFactorProperty, value); }
        public static readonly DependencyProperty IncrementFactorProperty = DependencyProperty.Register("IncrementFactor", typeof(double), typeof(IntEditor), new PropertyMetadata(0.1));

        public event PropertyChangedExtendedEventHandler<int> ValueChanged;

        static IntEditor()
        {
            DragProperty.OverrideMetadata(typeof(IntEditor), new PropertyMetadata(true));
            TextProperty.OverrideMetadata(typeof(IntEditor), new PropertyMetadata("0"));
            TextValidationProperty.OverrideMetadata(typeof(IntEditor), new PropertyMetadata(new Predicate<string>(s => s.IsInt().ShowException())));
        }
        public IntEditor()
        {
            ContentType = ContentType.Integer;
            Desactivated += (sender, e) => Value = Text.ToInt() ?? Value;
        }

        protected override void OnIncrement(double value)
        {
            if (DragValue == value) m_baseValue = Value;
            int v = m_baseValue + (int)(DragValue * IncrementFactor);
            Value = IsUnsigned ? Max(0, v) : v;
        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.Property == ValueProperty)
            {
                Text = ((int)e.NewValue).ToString();
                ValueChanged?.Invoke(this, new PropertyChangedExtendedEventArgs<int>("Value", (int)e.OldValue, (int)e.NewValue));
            }
            else if (e.Property == IsUnsignedProperty)
            {
                if ((bool)e.NewValue)
                {
                    ContentType = ContentType.UnsignedInteger;
                    TextValidation = s => s.IsInt(out int i).ShowException() && i >= 0;
                }
                else
                {
                    ContentType = ContentType.Integer;
                    TextValidation = s => s.IsInt().ShowException();
                }
            }
            base.OnPropertyChanged(e);
        }
    }

    public class UIntEditor : SwitchableTextBox
    {
        private uint m_baseValue;

        public uint Value { get => (uint)GetValue(ValueProperty); set => SetValue(ValueProperty, value); }
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(uint), typeof(UIntEditor));

        public double IncrementFactor { get => (double)GetValue(IncrementFactorProperty); set => SetValue(IncrementFactorProperty, value); }
        public static readonly DependencyProperty IncrementFactorProperty = DependencyProperty.Register("IncrementFactor", typeof(double), typeof(UIntEditor), new PropertyMetadata(0.1));

        public event PropertyChangedExtendedEventHandler<uint> ValueChanged;

        static UIntEditor()
        {
            DragProperty.OverrideMetadata(typeof(UIntEditor), new PropertyMetadata(true));
            TextProperty.OverrideMetadata(typeof(UIntEditor), new PropertyMetadata("0"));
            TextValidationProperty.OverrideMetadata(typeof(UIntEditor), new PropertyMetadata(new Predicate<string>(s => s.IsUInt().ShowException())));
        }
        public UIntEditor()
        {
            ContentType = ContentType.UnsignedInteger;
            Desactivated += (sender, e) => Value = Text.ToUInt() ?? Value;
        }

        protected override void OnIncrement(double value)
        {
            if (DragValue == value) m_baseValue = Value;
            Value = Max(0, m_baseValue + (uint)(DragValue * IncrementFactor));
        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.Property == ValueProperty)
            {
                Text = ((uint)e.NewValue).ToString();
                ValueChanged?.Invoke(this, new PropertyChangedExtendedEventArgs<uint>("Value", (uint)e.OldValue, (uint)e.NewValue));
            }
            base.OnPropertyChanged(e);
        }
    }

    public class LongEditor : SwitchableTextBox
    {
        private long m_baseValue;

        public long Value { get => (long)GetValue(ValueProperty); set => SetValue(ValueProperty, value); }
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(long), typeof(LongEditor));

        public bool IsUnsigned { get => (bool)GetValue(IsUnsignedProperty); set => SetValue(IsUnsignedProperty, value); }
        public static readonly DependencyProperty IsUnsignedProperty = DependencyProperty.Register("IsUnsigned", typeof(bool), typeof(LongEditor));

        public double IncrementFactor { get => (double)GetValue(IncrementFactorProperty); set => SetValue(IncrementFactorProperty, value); }
        public static readonly DependencyProperty IncrementFactorProperty = DependencyProperty.Register("IncrementFactor", typeof(double), typeof(LongEditor), new PropertyMetadata(0.1));

        public event PropertyChangedExtendedEventHandler<long> ValueChanged;

        static LongEditor()
        {
            DragProperty.OverrideMetadata(typeof(LongEditor), new PropertyMetadata(true));
            TextProperty.OverrideMetadata(typeof(LongEditor), new PropertyMetadata("0"));
            TextValidationProperty.OverrideMetadata(typeof(LongEditor), new PropertyMetadata(new Predicate<string>(s => s.IsLong().ShowException())));
        }
        public LongEditor()
        {
            ContentType = ContentType.Integer;
            Desactivated += (sender, e) => Value = Text.ToLong() ?? Value;
        }

        protected override void OnIncrement(double value)
        {
            if (DragValue == value) m_baseValue = Value;
            long v = m_baseValue + (long)(DragValue * IncrementFactor);
            Value = IsUnsigned ? Max(0, v) : v;
        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.Property == ValueProperty)
            {
                Text = ((long)e.NewValue).ToString();
                ValueChanged?.Invoke(this, new PropertyChangedExtendedEventArgs<long>("Value", (long)e.OldValue, (long)e.NewValue));
            }
            else if (e.Property == IsUnsignedProperty)
            {
                if ((bool)e.NewValue)
                {
                    ContentType = ContentType.UnsignedInteger;
                    TextValidation = s => s.IsLong(out long l).ShowException() && l >= 0;
                }
                else
                {
                    ContentType = ContentType.Integer;
                    TextValidation = s => s.IsLong().ShowException();
                }
            }
            base.OnPropertyChanged(e);
        }
    }

    public class ULongEditor : SwitchableTextBox
    {
        private ulong m_baseValue;

        public ulong Value { get => (ulong)GetValue(ValueProperty); set => SetValue(ValueProperty, value); }
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(ulong), typeof(ULongEditor));

        public double IncrementFactor { get => (double)GetValue(IncrementFactorProperty); set => SetValue(IncrementFactorProperty, value); }
        public static readonly DependencyProperty IncrementFactorProperty = DependencyProperty.Register("IncrementFactor", typeof(double), typeof(ULongEditor), new PropertyMetadata(0.1));

        public event PropertyChangedExtendedEventHandler<ulong> ValueChanged;

        static ULongEditor()
        {
            DragProperty.OverrideMetadata(typeof(ULongEditor), new PropertyMetadata(true));
            TextProperty.OverrideMetadata(typeof(ULongEditor), new PropertyMetadata("0"));
            TextValidationProperty.OverrideMetadata(typeof(ULongEditor), new PropertyMetadata(new Predicate<string>(s => s.IsULong().ShowException())));
        }
        public ULongEditor()
        {
            ContentType = ContentType.UnsignedInteger;
            Desactivated += (sender, e) => Value = Text.ToULong() ?? Value;
        }

        protected override void OnIncrement(double value)
        {
            if (DragValue == value) m_baseValue = Value;
            Value = Max(0, m_baseValue + (ulong)(DragValue * IncrementFactor));
        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.Property == ValueProperty)
            {
                Text = ((ulong)e.NewValue).ToString();
                ValueChanged?.Invoke(this, new PropertyChangedExtendedEventArgs<ulong>("Value", (ulong)e.OldValue, (ulong)e.NewValue));
            }
            base.OnPropertyChanged(e);
        }
    }

    public class ShortEditor : SwitchableTextBox
    {
        private short m_baseValue;

        public short Value { get => (short)GetValue(ValueProperty); set => SetValue(ValueProperty, value); }
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(short), typeof(ShortEditor));

        public bool IsUnsigned { get => (bool)GetValue(IsUnsignedProperty); set => SetValue(IsUnsignedProperty, value); }
        public static readonly DependencyProperty IsUnsignedProperty = DependencyProperty.Register("IsUnsigned", typeof(bool), typeof(ShortEditor));

        public double IncrementFactor { get => (double)GetValue(IncrementFactorProperty); set => SetValue(IncrementFactorProperty, value); }
        public static readonly DependencyProperty IncrementFactorProperty = DependencyProperty.Register("IncrementFactor", typeof(double), typeof(ShortEditor), new PropertyMetadata(0.1));

        public event PropertyChangedExtendedEventHandler<short> ValueChanged;

        static ShortEditor()
        {
            DragProperty.OverrideMetadata(typeof(ShortEditor), new PropertyMetadata(true));
            TextProperty.OverrideMetadata(typeof(ShortEditor), new PropertyMetadata("0"));
            TextValidationProperty.OverrideMetadata(typeof(ShortEditor), new PropertyMetadata(new Predicate<string>(s => s.IsShort().ShowException())));
        }
        public ShortEditor()
        {
            ContentType = ContentType.Integer;
            Desactivated += (sender, e) => Value = Text.ToShort() ?? Value;
        }

        protected override void OnIncrement(double value)
        {
            if (DragValue == value) m_baseValue = Value;
            short v = (short)(m_baseValue + (short)(DragValue * IncrementFactor));
            Value = IsUnsigned ? Max((short)0, v) : v;
        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.Property == ValueProperty)
            {
                Text = ((short)e.NewValue).ToString();
                ValueChanged?.Invoke(this, new PropertyChangedExtendedEventArgs<short>("Value", (short)e.OldValue, (short)e.NewValue));
            }
            else if (e.Property == IsUnsignedProperty)
            {
                if ((bool)e.NewValue)
                {
                    ContentType = ContentType.UnsignedInteger;
                    TextValidation = s => s.IsShort(out short sh).ShowException() && sh >= 0;
                }
                else
                {
                    ContentType = ContentType.Integer;
                    TextValidation = s => s.IsShort().ShowException();
                }
            }
            base.OnPropertyChanged(e);
        }
    }

    public class UShortEditor : SwitchableTextBox
    {
        private ushort m_baseValue;

        public ushort Value { get => (ushort)GetValue(ValueProperty); set => SetValue(ValueProperty, value); }
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(ushort), typeof(UShortEditor));

        public double IncrementFactor { get => (double)GetValue(IncrementFactorProperty); set => SetValue(IncrementFactorProperty, value); }
        public static readonly DependencyProperty IncrementFactorProperty = DependencyProperty.Register("IncrementFactor", typeof(double), typeof(UShortEditor), new PropertyMetadata(0.1));

        public event PropertyChangedExtendedEventHandler<ushort> ValueChanged;

        static UShortEditor()
        {
            DragProperty.OverrideMetadata(typeof(UShortEditor), new PropertyMetadata(true));
            TextProperty.OverrideMetadata(typeof(UShortEditor), new PropertyMetadata("0"));
            TextValidationProperty.OverrideMetadata(typeof(UShortEditor), new PropertyMetadata(new Predicate<string>(s => s.IsUShort().ShowException())));
        }
        public UShortEditor()
        {
            ContentType = ContentType.UnsignedInteger;
            Desactivated += (sender, e) => Value = Text.ToUShort() ?? Value;
        }

        protected override void OnIncrement(double value)
        {
            if (DragValue == value) m_baseValue = Value;
            Value = Max((ushort)0, (ushort)(m_baseValue + (ushort)(DragValue * IncrementFactor)));
        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.Property == ValueProperty)
            {
                Text = ((ushort)e.NewValue).ToString();
                ValueChanged?.Invoke(this, new PropertyChangedExtendedEventArgs<ushort>("Value", (ushort)e.OldValue, (ushort)e.NewValue));
            }
            base.OnPropertyChanged(e);
        }
    }

    public class ByteEditor : SwitchableTextBox
    {
        private byte m_baseValue;

        public byte Value { get => (byte)GetValue(ValueProperty); set => SetValue(ValueProperty, value); }
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(byte), typeof(ByteEditor));

        public double IncrementFactor { get => (double)GetValue(IncrementFactorProperty); set => SetValue(IncrementFactorProperty, value); }
        public static readonly DependencyProperty IncrementFactorProperty = DependencyProperty.Register("IncrementFactor", typeof(double), typeof(ByteEditor), new PropertyMetadata(0.1));

        public event PropertyChangedExtendedEventHandler<byte> ValueChanged;

        static ByteEditor()
        {
            DragProperty.OverrideMetadata(typeof(ByteEditor), new PropertyMetadata(true));
            TextProperty.OverrideMetadata(typeof(ByteEditor), new PropertyMetadata("0"));
            TextValidationProperty.OverrideMetadata(typeof(ByteEditor), new PropertyMetadata(new Predicate<string>(s => s.IsByte().ShowException())));
        }
        public ByteEditor()
        {
            ContentType = ContentType.UnsignedInteger;
            Desactivated += (sender, e) => Value = Text.ToByte() ?? Value;
        }

        protected override void OnIncrement(double value)
        {
            if (DragValue == value) m_baseValue = Value;
            Value = (byte)Max(0, m_baseValue + (byte)(DragValue * IncrementFactor));
        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.Property == ValueProperty)
            {
                Text = ((byte)e.NewValue).ToString();
                ValueChanged?.Invoke(this, new PropertyChangedExtendedEventArgs<byte>("Value", (byte)e.OldValue, (byte)e.NewValue));
            }
            base.OnPropertyChanged(e);
        }
    }

    public class SByteEditor : SwitchableTextBox
    {
        private sbyte m_baseValue;

        public sbyte Value { get => (sbyte)GetValue(ValueProperty); set => SetValue(ValueProperty, value); }
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(sbyte), typeof(SByteEditor));

        public bool IsUnsigned { get => (bool)GetValue(IsUnsignedProperty); set => SetValue(IsUnsignedProperty, value); }
        public static readonly DependencyProperty IsUnsignedProperty = DependencyProperty.Register("IsUnsigned", typeof(bool), typeof(SByteEditor));

        public double IncrementFactor { get => (double)GetValue(IncrementFactorProperty); set => SetValue(IncrementFactorProperty, value); }
        public static readonly DependencyProperty IncrementFactorProperty = DependencyProperty.Register("IncrementFactor", typeof(double), typeof(SByteEditor), new PropertyMetadata(0.1));

        public event PropertyChangedExtendedEventHandler<sbyte> ValueChanged;

        static SByteEditor()
        {
            DragProperty.OverrideMetadata(typeof(SByteEditor), new PropertyMetadata(true));
            TextProperty.OverrideMetadata(typeof(SByteEditor), new PropertyMetadata("0"));
            TextValidationProperty.OverrideMetadata(typeof(SByteEditor), new PropertyMetadata(new Predicate<string>(s => s.IsSByte().ShowException())));
        }
        public SByteEditor()
        {
            ContentType = ContentType.Integer;
            Desactivated += (sender, e) => Value = Text.ToSByte() ?? Value;
        }

        protected override void OnIncrement(double value)
        {
            if (DragValue == value) m_baseValue = Value;
            sbyte v = (sbyte)(m_baseValue + (sbyte)(DragValue * IncrementFactor));
            Value = IsUnsigned ? Max((sbyte)0, v) : v;
        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.Property == ValueProperty)
            {
                Text = ((sbyte)e.NewValue).ToString();
                ValueChanged?.Invoke(this, new PropertyChangedExtendedEventArgs<sbyte>("Value", (sbyte)e.OldValue, (sbyte)e.NewValue));
            }
            else if (e.Property == IsUnsignedProperty)
            {
                if ((bool)e.NewValue)
                {
                    ContentType = ContentType.UnsignedInteger;
                    TextValidation = s => s.IsSByte(out sbyte b).ShowException() && b >= 0;
                }
                else
                {
                    ContentType = ContentType.Integer;
                    TextValidation = s => s.IsSByte().ShowException();
                }
            }
            base.OnPropertyChanged(e);
        }
    }
}
