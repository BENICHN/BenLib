using BenLib.Framework;
using BenLib.Standard;
using System.Windows;
using System.Windows.Input;
using static System.Math;

namespace BenLib.WPF
{
    public class DoubleEditor : SwitchableTextBox
    {
        private double m_baseValue;

        public double Value { get => (double)GetValue(ValueProperty); set => SetValue(ValueProperty, value); }
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(double), typeof(DoubleEditor));

        public double MinValue { get => (double)GetValue(MinValueProperty); set => SetValue(MinValueProperty, value); }
        public static readonly DependencyProperty MinValueProperty = DependencyProperty.Register("MinValue", typeof(double), typeof(DoubleEditor), new PropertyMetadata(double.NegativeInfinity));

        public double MaxValue { get => (double)GetValue(MaxValueProperty); set => SetValue(MaxValueProperty, value); }
        public static readonly DependencyProperty MaxValueProperty = DependencyProperty.Register("MaxValue", typeof(double), typeof(DoubleEditor), new PropertyMetadata(double.PositiveInfinity));

        public double IncrementFactor { get => (double)GetValue(IncrementFactorProperty); set => SetValue(IncrementFactorProperty, value); }
        public static readonly DependencyProperty IncrementFactorProperty = DependencyProperty.Register("IncrementFactor", typeof(double), typeof(DoubleEditor), new PropertyMetadata(0.1));

        public event PropertyChangedExtendedEventHandler<double> ValueChanged;

        static DoubleEditor()
        {
            DragProperty.OverrideMetadata(typeof(DoubleEditor), new PropertyMetadata(true));
            TextProperty.OverrideMetadata(typeof(DoubleEditor), new PropertyMetadata("0"));
        }
        public DoubleEditor()
        {
            AllowedStrings = new[] { "-∞", "∞", "NaN" };
            TextValidation = s => s.IsDouble(out double d).ShowException() && MinValue <= d && d <= MaxValue;
            ContentType = ContentType.Decimal;
            Desactivated += (sender, e) => Value = Text.ToDouble() ?? Value;
        }

        protected override void OnIncrement(double value)
        {
            if (DragValue == value) m_baseValue = Value;
            double v = m_baseValue + DragValue * IncrementFactor;
            Value = Round(v.Trim(MinValue, MaxValue), 4);
        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.Property == ValueProperty)
            {
                Text = ((double)e.NewValue).ToString();
                ValueChanged?.Invoke(this, new PropertyChangedExtendedEventArgs<double>("Value", (double)e.OldValue, (double)e.NewValue));
            }
            else if (e.Property == MinValueProperty) ContentType = (double)e.NewValue >= 0 ? ContentType.UnsignedDecimal : ContentType.Decimal;
            base.OnPropertyChanged(e);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Oem102:
                    Value = Input.IsShiftPressed() ? double.PositiveInfinity : double.NegativeInfinity;
                    break;
                case Key.N:
                    Value = double.NaN;
                    break;
            }
        }
    }

    public class FloatEditor : SwitchableTextBox
    {
        private float m_baseValue;

        public float Value { get => (float)GetValue(ValueProperty); set => SetValue(ValueProperty, value); }
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(float), typeof(FloatEditor));

        public float MinValue { get => (float)GetValue(MinValueProperty); set => SetValue(MinValueProperty, value); }
        public static readonly DependencyProperty MinValueProperty = DependencyProperty.Register("MinValue", typeof(float), typeof(FloatEditor), new PropertyMetadata(float.NegativeInfinity));

        public float MaxValue { get => (float)GetValue(MaxValueProperty); set => SetValue(MaxValueProperty, value); }
        public static readonly DependencyProperty MaxValueProperty = DependencyProperty.Register("MaxValue", typeof(float), typeof(FloatEditor), new PropertyMetadata(float.PositiveInfinity));

        public double IncrementFactor { get => (double)GetValue(IncrementFactorProperty); set => SetValue(IncrementFactorProperty, value); }
        public static readonly DependencyProperty IncrementFactorProperty = DependencyProperty.Register("IncrementFactor", typeof(double), typeof(FloatEditor), new PropertyMetadata(0.1));

        public event PropertyChangedExtendedEventHandler<float> ValueChanged;

        static FloatEditor()
        {
            DragProperty.OverrideMetadata(typeof(FloatEditor), new PropertyMetadata(true));
            TextProperty.OverrideMetadata(typeof(FloatEditor), new PropertyMetadata("0"));
        }
        public FloatEditor()
        {
            AllowedStrings = new[] { "-∞", "∞", "NaN" };
            TextValidation = s => s.IsFloat(out float f).ShowException() && MinValue <= f && f <= MaxValue;
            ContentType = ContentType.Decimal;
            Desactivated += (sender, e) => Value = Text.ToFloat() ?? Value;
        }

        protected override void OnIncrement(double value)
        {
            if (DragValue == value) m_baseValue = Value;
            float v = m_baseValue + (float)(DragValue * IncrementFactor);
            Value = v.Trim(MinValue, MaxValue);
        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.Property == ValueProperty)
            {
                Text = ((float)e.NewValue).ToString();
                ValueChanged?.Invoke(this, new PropertyChangedExtendedEventArgs<float>("Value", (float)e.OldValue, (float)e.NewValue));
            }
            else if (e.Property == MinValueProperty) ContentType = (float)e.NewValue >= 0 ? ContentType.UnsignedDecimal : ContentType.Decimal;
            base.OnPropertyChanged(e);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Oem102:
                    Value = Input.IsShiftPressed() ? float.PositiveInfinity : float.NegativeInfinity;
                    break;
                case Key.N:
                    Value = float.NaN;
                    break;
            }
        }
    }

    public class DecimalEditor : SwitchableTextBox
    {
        private decimal m_baseValue;

        public decimal Value { get => (decimal)GetValue(ValueProperty); set => SetValue(ValueProperty, value); }
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(decimal), typeof(DecimalEditor));

        public decimal MinValue { get => (decimal)GetValue(MinValueProperty); set => SetValue(MinValueProperty, value); }
        public static readonly DependencyProperty MinValueProperty = DependencyProperty.Register("MinValue", typeof(decimal), typeof(DecimalEditor), new PropertyMetadata(decimal.MinValue));

        public decimal MaxValue { get => (decimal)GetValue(MaxValueProperty); set => SetValue(MaxValueProperty, value); }
        public static readonly DependencyProperty MaxValueProperty = DependencyProperty.Register("MaxValue", typeof(decimal), typeof(DecimalEditor), new PropertyMetadata(decimal.MaxValue));

        public double IncrementFactor { get => (double)GetValue(IncrementFactorProperty); set => SetValue(IncrementFactorProperty, value); }
        public static readonly DependencyProperty IncrementFactorProperty = DependencyProperty.Register("IncrementFactor", typeof(double), typeof(DecimalEditor), new PropertyMetadata(0.1));

        public event PropertyChangedExtendedEventHandler<decimal> ValueChanged;

        static DecimalEditor()
        {
            DragProperty.OverrideMetadata(typeof(DecimalEditor), new PropertyMetadata(true));
            TextProperty.OverrideMetadata(typeof(DecimalEditor), new PropertyMetadata("0"));
        }
        public DecimalEditor()
        {
            TextValidation = s => s.IsDecimal(out decimal m).ShowException() && MinValue <= m && m <= MaxValue;
            ContentType = ContentType.Decimal;
            Desactivated += (sender, e) => Value = Text.ToDecimal() ?? Value;
        }

        protected override void OnIncrement(double value)
        {
            if (DragValue == value) m_baseValue = Value;
            decimal v = m_baseValue + (decimal)(DragValue * IncrementFactor);
            Value = Round(v.Trim(MinValue, MaxValue), 4);
        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.Property == ValueProperty)
            {
                Text = ((decimal)e.NewValue).ToString();
                ValueChanged?.Invoke(this, new PropertyChangedExtendedEventArgs<decimal>("Value", (decimal)e.OldValue, (decimal)e.NewValue));
            }
            else if (e.Property == MinValueProperty) ContentType = (decimal)e.NewValue >= 0 ? ContentType.UnsignedDecimal : ContentType.Decimal;
            base.OnPropertyChanged(e);
        }
    }

    public class IntEditor : SwitchableTextBox
    {
        private int m_baseValue;

        public int Value { get => (int)GetValue(ValueProperty); set => SetValue(ValueProperty, value); }
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(int), typeof(IntEditor));

        public int MinValue { get => (int)GetValue(MinValueProperty); set => SetValue(MinValueProperty, value); }
        public static readonly DependencyProperty MinValueProperty = DependencyProperty.Register("MinValue", typeof(int), typeof(IntEditor), new PropertyMetadata(int.MinValue));

        public int MaxValue { get => (int)GetValue(MaxValueProperty); set => SetValue(MaxValueProperty, value); }
        public static readonly DependencyProperty MaxValueProperty = DependencyProperty.Register("MaxValue", typeof(int), typeof(IntEditor), new PropertyMetadata(int.MaxValue));

        public double IncrementFactor { get => (double)GetValue(IncrementFactorProperty); set => SetValue(IncrementFactorProperty, value); }
        public static readonly DependencyProperty IncrementFactorProperty = DependencyProperty.Register("IncrementFactor", typeof(double), typeof(IntEditor), new PropertyMetadata(0.1));

        public event PropertyChangedExtendedEventHandler<int> ValueChanged;

        static IntEditor()
        {
            DragProperty.OverrideMetadata(typeof(IntEditor), new PropertyMetadata(true));
            TextProperty.OverrideMetadata(typeof(IntEditor), new PropertyMetadata("0"));
        }
        public IntEditor()
        {
            TextValidation = s => s.IsInt(out int i).ShowException() && MinValue <= i && i <= MaxValue;
            ContentType = ContentType.Integer;
            Desactivated += (sender, e) => Value = Text.ToInt() ?? Value;
        }

        protected override void OnIncrement(double value)
        {
            if (DragValue == value) m_baseValue = Value;
            int v = m_baseValue + (int)(DragValue * IncrementFactor);
            Value = v.Trim(MinValue, MaxValue);
        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.Property == ValueProperty)
            {
                Text = ((int)e.NewValue).ToString();
                ValueChanged?.Invoke(this, new PropertyChangedExtendedEventArgs<int>("Value", (int)e.OldValue, (int)e.NewValue));
            }
            else if (e.Property == MinValueProperty) ContentType = (int)e.NewValue >= 0 ? ContentType.UnsignedInteger : ContentType.Integer;
            base.OnPropertyChanged(e);
        }
    }

    public class UIntEditor : SwitchableTextBox
    {
        private uint m_baseValue;

        public uint Value { get => (uint)GetValue(ValueProperty); set => SetValue(ValueProperty, value); }
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(uint), typeof(UIntEditor));

        public uint MinValue { get => (uint)GetValue(MinValueProperty); set => SetValue(MinValueProperty, value); }
        public static readonly DependencyProperty MinValueProperty = DependencyProperty.Register("MinValue", typeof(uint), typeof(UIntEditor), new PropertyMetadata(uint.MinValue));

        public uint MaxValue { get => (uint)GetValue(MaxValueProperty); set => SetValue(MaxValueProperty, value); }
        public static readonly DependencyProperty MaxValueProperty = DependencyProperty.Register("MaxValue", typeof(uint), typeof(UIntEditor), new PropertyMetadata(uint.MaxValue));

        public double IncrementFactor { get => (double)GetValue(IncrementFactorProperty); set => SetValue(IncrementFactorProperty, value); }
        public static readonly DependencyProperty IncrementFactorProperty = DependencyProperty.Register("IncrementFactor", typeof(double), typeof(UIntEditor), new PropertyMetadata(0.1));

        public event PropertyChangedExtendedEventHandler<uint> ValueChanged;

        static UIntEditor()
        {
            DragProperty.OverrideMetadata(typeof(UIntEditor), new PropertyMetadata(true));
            TextProperty.OverrideMetadata(typeof(UIntEditor), new PropertyMetadata("0"));
        }
        public UIntEditor()
        {
            TextValidation = s => s.IsUInt(out uint ui).ShowException() && MinValue <= ui && ui <= MaxValue;
            ContentType = ContentType.UnsignedInteger;
            Desactivated += (sender, e) => Value = Text.ToUInt() ?? Value;
        }

        protected override void OnIncrement(double value)
        {
            if (DragValue == value) m_baseValue = Value;
            uint v = m_baseValue + (uint)(DragValue * IncrementFactor);
            Value = v.Trim(MinValue, MaxValue);
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

        public long MinValue { get => (long)GetValue(MinValueProperty); set => SetValue(MinValueProperty, value); }
        public static readonly DependencyProperty MinValueProperty = DependencyProperty.Register("MinValue", typeof(long), typeof(LongEditor), new PropertyMetadata(long.MinValue));

        public long MaxValue { get => (long)GetValue(MaxValueProperty); set => SetValue(MaxValueProperty, value); }
        public static readonly DependencyProperty MaxValueProperty = DependencyProperty.Register("MaxValue", typeof(long), typeof(LongEditor), new PropertyMetadata(long.MaxValue));

        public double IncrementFactor { get => (double)GetValue(IncrementFactorProperty); set => SetValue(IncrementFactorProperty, value); }
        public static readonly DependencyProperty IncrementFactorProperty = DependencyProperty.Register("IncrementFactor", typeof(double), typeof(LongEditor), new PropertyMetadata(0.1));

        public event PropertyChangedExtendedEventHandler<long> ValueChanged;

        static LongEditor()
        {
            DragProperty.OverrideMetadata(typeof(LongEditor), new PropertyMetadata(true));
            TextProperty.OverrideMetadata(typeof(LongEditor), new PropertyMetadata("0"));
        }
        public LongEditor()
        {
            TextValidation = s => s.IsLong(out long l).ShowException() && MinValue <= l && l <= MaxValue;
            ContentType = ContentType.Integer;
            Desactivated += (sender, e) => Value = Text.ToLong() ?? Value;
        }

        protected override void OnIncrement(double value)
        {
            if (DragValue == value) m_baseValue = Value;
            long v = m_baseValue + (long)(DragValue * IncrementFactor);
            Value = v.Trim(MinValue, MaxValue);
        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.Property == ValueProperty)
            {
                Text = ((long)e.NewValue).ToString();
                ValueChanged?.Invoke(this, new PropertyChangedExtendedEventArgs<long>("Value", (long)e.OldValue, (long)e.NewValue));
            }
            else if (e.Property == MinValueProperty) ContentType = (long)e.NewValue >= 0 ? ContentType.UnsignedInteger : ContentType.Integer;
            base.OnPropertyChanged(e);
        }
    }

    public class ULongEditor : SwitchableTextBox
    {
        private ulong m_baseValue;

        public ulong Value { get => (ulong)GetValue(ValueProperty); set => SetValue(ValueProperty, value); }
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(ulong), typeof(ULongEditor));

        public ulong MinValue { get => (ulong)GetValue(MinValueProperty); set => SetValue(MinValueProperty, value); }
        public static readonly DependencyProperty MinValueProperty = DependencyProperty.Register("MinValue", typeof(ulong), typeof(ULongEditor), new PropertyMetadata(ulong.MinValue));

        public ulong MaxValue { get => (ulong)GetValue(MaxValueProperty); set => SetValue(MaxValueProperty, value); }
        public static readonly DependencyProperty MaxValueProperty = DependencyProperty.Register("MaxValue", typeof(ulong), typeof(ULongEditor), new PropertyMetadata(ulong.MaxValue));

        public double IncrementFactor { get => (double)GetValue(IncrementFactorProperty); set => SetValue(IncrementFactorProperty, value); }
        public static readonly DependencyProperty IncrementFactorProperty = DependencyProperty.Register("IncrementFactor", typeof(double), typeof(ULongEditor), new PropertyMetadata(0.1));

        public event PropertyChangedExtendedEventHandler<ulong> ValueChanged;

        static ULongEditor()
        {
            DragProperty.OverrideMetadata(typeof(ULongEditor), new PropertyMetadata(true));
            TextProperty.OverrideMetadata(typeof(ULongEditor), new PropertyMetadata("0"));
        }
        public ULongEditor()
        {
            TextValidation = s => s.IsULong(out ulong ul).ShowException() && MinValue <= ul && ul <= MaxValue;
            ContentType = ContentType.UnsignedInteger;
            Desactivated += (sender, e) => Value = Text.ToULong() ?? Value;
        }

        protected override void OnIncrement(double value)
        {
            if (DragValue == value) m_baseValue = Value;
            ulong v = m_baseValue + (ulong)(DragValue * IncrementFactor);
            Value = v.Trim(MinValue, MaxValue);
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

        public short MinValue { get => (short)GetValue(MinValueProperty); set => SetValue(MinValueProperty, value); }
        public static readonly DependencyProperty MinValueProperty = DependencyProperty.Register("MinValue", typeof(short), typeof(ShortEditor), new PropertyMetadata(short.MinValue));

        public short MaxValue { get => (short)GetValue(MaxValueProperty); set => SetValue(MaxValueProperty, value); }
        public static readonly DependencyProperty MaxValueProperty = DependencyProperty.Register("MaxValue", typeof(short), typeof(ShortEditor), new PropertyMetadata(short.MaxValue));

        public double IncrementFactor { get => (double)GetValue(IncrementFactorProperty); set => SetValue(IncrementFactorProperty, value); }
        public static readonly DependencyProperty IncrementFactorProperty = DependencyProperty.Register("IncrementFactor", typeof(double), typeof(ShortEditor), new PropertyMetadata(0.1));

        public event PropertyChangedExtendedEventHandler<short> ValueChanged;

        static ShortEditor()
        {
            DragProperty.OverrideMetadata(typeof(ShortEditor), new PropertyMetadata(true));
            TextProperty.OverrideMetadata(typeof(ShortEditor), new PropertyMetadata("0"));
        }
        public ShortEditor()
        {
            TextValidation = s => s.IsShort(out short sh).ShowException() && MinValue <= sh && sh <= MaxValue;
            ContentType = ContentType.Integer;
            Desactivated += (sender, e) => Value = Text.ToShort() ?? Value;
        }

        protected override void OnIncrement(double value)
        {
            if (DragValue == value) m_baseValue = Value;
            short v = (short)(m_baseValue + DragValue * IncrementFactor);
            Value = v.Trim(MinValue, MaxValue);
        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.Property == ValueProperty)
            {
                Text = ((short)e.NewValue).ToString();
                ValueChanged?.Invoke(this, new PropertyChangedExtendedEventArgs<short>("Value", (short)e.OldValue, (short)e.NewValue));
            }
            else if (e.Property == MinValueProperty) ContentType = (short)e.NewValue >= 0 ? ContentType.UnsignedInteger : ContentType.Integer;
            base.OnPropertyChanged(e);
        }
    }

    public class UShortEditor : SwitchableTextBox
    {
        private ushort m_baseValue;

        public ushort Value { get => (ushort)GetValue(ValueProperty); set => SetValue(ValueProperty, value); }
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(ushort), typeof(UShortEditor));

        public ushort MinValue { get => (ushort)GetValue(MinValueProperty); set => SetValue(MinValueProperty, value); }
        public static readonly DependencyProperty MinValueProperty = DependencyProperty.Register("MinValue", typeof(ushort), typeof(UShortEditor), new PropertyMetadata(ushort.MinValue));

        public ushort MaxValue { get => (ushort)GetValue(MaxValueProperty); set => SetValue(MaxValueProperty, value); }
        public static readonly DependencyProperty MaxValueProperty = DependencyProperty.Register("MaxValue", typeof(ushort), typeof(UShortEditor), new PropertyMetadata(ushort.MaxValue));

        public double IncrementFactor { get => (double)GetValue(IncrementFactorProperty); set => SetValue(IncrementFactorProperty, value); }
        public static readonly DependencyProperty IncrementFactorProperty = DependencyProperty.Register("IncrementFactor", typeof(double), typeof(UShortEditor), new PropertyMetadata(0.1));

        public event PropertyChangedExtendedEventHandler<ushort> ValueChanged;

        static UShortEditor()
        {
            DragProperty.OverrideMetadata(typeof(UShortEditor), new PropertyMetadata(true));
            TextProperty.OverrideMetadata(typeof(UShortEditor), new PropertyMetadata("0"));
        }
        public UShortEditor()
        {
            TextValidation = s => s.IsUShort(out ushort ush).ShowException() && MinValue <= ush && ush <= MaxValue;
            ContentType = ContentType.UnsignedInteger;
            Desactivated += (sender, e) => Value = Text.ToUShort() ?? Value;
        }

        protected override void OnIncrement(double value)
        {
            if (DragValue == value) m_baseValue = Value;
            ushort v = (ushort)(m_baseValue + DragValue * IncrementFactor);
            Value = v.Trim(MinValue, MaxValue);
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

        public byte MinValue { get => (byte)GetValue(MinValueProperty); set => SetValue(MinValueProperty, value); }
        public static readonly DependencyProperty MinValueProperty = DependencyProperty.Register("MinValue", typeof(byte), typeof(ByteEditor), new PropertyMetadata(byte.MinValue));

        public byte MaxValue { get => (byte)GetValue(MaxValueProperty); set => SetValue(MaxValueProperty, value); }
        public static readonly DependencyProperty MaxValueProperty = DependencyProperty.Register("MaxValue", typeof(byte), typeof(ByteEditor), new PropertyMetadata(byte.MaxValue));

        public double IncrementFactor { get => (double)GetValue(IncrementFactorProperty); set => SetValue(IncrementFactorProperty, value); }
        public static readonly DependencyProperty IncrementFactorProperty = DependencyProperty.Register("IncrementFactor", typeof(double), typeof(ByteEditor), new PropertyMetadata(0.1));

        public event PropertyChangedExtendedEventHandler<byte> ValueChanged;

        static ByteEditor()
        {
            DragProperty.OverrideMetadata(typeof(ByteEditor), new PropertyMetadata(true));
            TextProperty.OverrideMetadata(typeof(ByteEditor), new PropertyMetadata("0"));
        }
        public ByteEditor()
        {
            TextValidation = s => s.IsByte(out byte b).ShowException() && MinValue <= b && b <= MaxValue;
            ContentType = ContentType.UnsignedInteger;
            Desactivated += (sender, e) => Value = Text.ToByte() ?? Value;
        }

        protected override void OnIncrement(double value)
        {
            if (DragValue == value) m_baseValue = Value;
            byte v = (byte)(m_baseValue + DragValue * IncrementFactor);
            Value = v.Trim(MinValue, MaxValue);
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

        public sbyte MinValue { get => (sbyte)GetValue(MinValueProperty); set => SetValue(MinValueProperty, value); }
        public static readonly DependencyProperty MinValueProperty = DependencyProperty.Register("MinValue", typeof(sbyte), typeof(SByteEditor), new PropertyMetadata(sbyte.MinValue));

        public sbyte MaxValue { get => (sbyte)GetValue(MaxValueProperty); set => SetValue(MaxValueProperty, value); }
        public static readonly DependencyProperty MaxValueProperty = DependencyProperty.Register("MaxValue", typeof(sbyte), typeof(SByteEditor), new PropertyMetadata(sbyte.MaxValue));

        public double IncrementFactor { get => (double)GetValue(IncrementFactorProperty); set => SetValue(IncrementFactorProperty, value); }
        public static readonly DependencyProperty IncrementFactorProperty = DependencyProperty.Register("IncrementFactor", typeof(double), typeof(SByteEditor), new PropertyMetadata(0.1));

        public event PropertyChangedExtendedEventHandler<sbyte> ValueChanged;

        static SByteEditor()
        {
            DragProperty.OverrideMetadata(typeof(SByteEditor), new PropertyMetadata(true));
            TextProperty.OverrideMetadata(typeof(SByteEditor), new PropertyMetadata("0"));
        }
        public SByteEditor()
        {
            TextValidation = s => s.IsSByte(out sbyte sb).ShowException() && MinValue <= sb && sb <= MaxValue;
            ContentType = ContentType.Integer;
            Desactivated += (sender, e) => Value = Text.ToSByte() ?? Value;
        }

        protected override void OnIncrement(double value)
        {
            if (DragValue == value) m_baseValue = Value;
            sbyte v = (sbyte)(m_baseValue + DragValue * IncrementFactor);
            Value = v.Trim(MinValue, MaxValue);
        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.Property == ValueProperty)
            {
                Text = ((sbyte)e.NewValue).ToString();
                ValueChanged?.Invoke(this, new PropertyChangedExtendedEventArgs<sbyte>("Value", (sbyte)e.OldValue, (sbyte)e.NewValue));
            }
            else if (e.Property == MinValueProperty) ContentType = (sbyte)e.NewValue >= 0 ? ContentType.UnsignedInteger : ContentType.Integer;
            base.OnPropertyChanged(e);
        }
    }
}
