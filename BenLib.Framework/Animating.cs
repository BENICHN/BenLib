using BenLib.Standard;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace BenLib.Framework
{
    internal class IntValueInterpolationHelper : ValueInterpolationHelper<int> { protected override int InterpolateCore(int start, int end, double progress) => Num.Interpolate(start, end, progress); }
    internal class UIntValueInterpolationHelper : ValueInterpolationHelper<uint> { protected override uint InterpolateCore(uint start, uint end, double progress) => Num.Interpolate(start, end, progress); }
    internal class LongValueInterpolationHelper : ValueInterpolationHelper<long> { protected override long InterpolateCore(long start, long end, double progress) => Num.Interpolate(start, end, progress); }
    internal class ULongValueInterpolationHelper : ValueInterpolationHelper<ulong> { protected override ulong InterpolateCore(ulong start, ulong end, double progress) => Num.Interpolate(start, end, progress); }
    internal class ShortValueInterpolationHelper : ValueInterpolationHelper<short> { protected override short InterpolateCore(short start, short end, double progress) => Num.Interpolate(start, end, progress); }
    internal class UShortValueInterpolationHelper : ValueInterpolationHelper<ushort> { protected override ushort InterpolateCore(ushort start, ushort end, double progress) => Num.Interpolate(start, end, progress); }
    internal class ByteValueInterpolationHelper : ValueInterpolationHelper<byte> { protected override byte InterpolateCore(byte start, byte end, double progress) => Num.Interpolate(start, end, progress); }
    internal class SByteValueInterpolationHelper : ValueInterpolationHelper<sbyte> { protected override sbyte InterpolateCore(sbyte start, sbyte end, double progress) => Num.Interpolate(start, end, progress); }
    internal class FloatValueInterpolationHelper : ValueInterpolationHelper<float> { protected override float InterpolateCore(float start, float end, double progress) => Num.Interpolate(start, end, progress); }
    internal class DoubleValueInterpolationHelper : ValueInterpolationHelper<double> { protected override double InterpolateCore(double start, double end, double progress) => Num.Interpolate(start, end, progress); }
    internal class DecimalValueInterpolationHelper : ValueInterpolationHelper<decimal> { protected override decimal InterpolateCore(decimal start, decimal end, double progress) => Num.Interpolate(start, end, progress); }
    internal class ColorValueInterpolationHelper : ValueInterpolationHelper<Color> { protected override Color InterpolateCore(Color start, Color end, double progress) => NumFramework.Interpolate(start, end, progress); }
    internal class PointValueInterpolationHelper : ValueInterpolationHelper<Point> { protected override Point InterpolateCore(Point start, Point end, double progress) => NumFramework.Interpolate(start, end, progress); }
    internal class VectorValueInterpolationHelper : ValueInterpolationHelper<Vector> { protected override Vector InterpolateCore(Vector start, Vector end, double progress) => NumFramework.Interpolate(start, end, progress); }
    internal class SizeValueInterpolationHelper : ValueInterpolationHelper<Size> { protected override Size InterpolateCore(Size start, Size end, double progress) => NumFramework.Interpolate(start, end, progress); }
    internal class RectValueInterpolationHelper : ValueInterpolationHelper<Rect> { protected override Rect InterpolateCore(Rect start, Rect end, double progress) => NumFramework.Interpolate(start, end, progress); }

    public class ValueInterpolationHelper<T>
    {
        private static ValueInterpolationHelper<T> m_default;
        public static ValueInterpolationHelper<T> Default { get => m_default ?? CreateDefault(); set => m_default = value; }

        public T Interpolate(T start, T end, double progress) => progress switch
        {
            0.0 => start,
            1.0 => end,
            _ => InterpolateCore(start, end, progress)
        };
        protected virtual T InterpolateCore(T start, T end, double progress) => progress < 1.0 ? start : end;

        public T Iterate(T value, T start, T end, long iterationsCount) => Interpolate(Interpolate(default, value, 2), Interpolate(start, end, 2 * iterationsCount), 0.5);

        private static ValueInterpolationHelper<T> CreateDefault()
        {
            var t = typeof(T);
            RuntimeHelpers.RunClassConstructor(t.TypeHandle);
            return m_default ?? (ValueInterpolationHelper<T>)(
                t == typeof(int) ? new IntValueInterpolationHelper() :
                t == typeof(uint) ? new UIntValueInterpolationHelper() :
                t == typeof(long) ? new LongValueInterpolationHelper() :
                t == typeof(ulong) ? new ULongValueInterpolationHelper() :
                t == typeof(short) ? new ShortValueInterpolationHelper() :
                t == typeof(ushort) ? new UShortValueInterpolationHelper() :
                t == typeof(byte) ? new ByteValueInterpolationHelper() :
                t == typeof(sbyte) ? new SByteValueInterpolationHelper() :
                t == typeof(float) ? new FloatValueInterpolationHelper() :
                t == typeof(double) ? new DoubleValueInterpolationHelper() :
                t == typeof(decimal) ? new DecimalValueInterpolationHelper() :
                t == typeof(Color) ? new ColorValueInterpolationHelper() :
                t == typeof(Point) ? new PointValueInterpolationHelper() :
                t == typeof(Vector) ? new VectorValueInterpolationHelper() :
                t == typeof(Size) ? new SizeValueInterpolationHelper() :
                t == typeof(Rect) ? new RectValueInterpolationHelper() :
                (object)new ValueInterpolationHelper<T>());
        }
    }

    public class EasingKeyFrame<T> : KeyFrame<T>
    {
        public IEasingFunction EasingFunction { get => (IEasingFunction)GetValue(EasingFunctionProperty); set => SetValue(EasingFunctionProperty, value); }
        public static readonly DependencyProperty EasingFunctionProperty = DependencyProperty.Register("EasingFunction", typeof(IEasingFunction), typeof(EasingKeyFrame<T>));
        protected override Freezable CreateInstanceCore() => new EasingKeyFrame<T>();
        protected override T InterpolateValueCore(T baseValue, double keyFrameProgress) => ValueInterpolationHelper<T>.Default.Interpolate(baseValue, Value, EasingFunction?.Ease(keyFrameProgress) ?? keyFrameProgress);
    }

    public class SplineKeyFrame<T> : KeyFrame<T>
    {
        public KeySpline KeySpline { get => (KeySpline)GetValue(KeySplineProperty); set => SetValue(KeySplineProperty, value); }
        public static readonly DependencyProperty KeySplineProperty = DependencyProperty.Register("KeySpline", typeof(KeySpline), typeof(SplineKeyFrame<T>), new PropertyMetadata(new KeySpline()), v => v != null);

        protected override Freezable CreateInstanceCore() => new SplineKeyFrame<T>();
        protected override T InterpolateValueCore(T baseValue, double keyFrameProgress) => ValueInterpolationHelper<T>.Default.Interpolate(baseValue, Value, KeySpline.GetSplineProgress(keyFrameProgress));
    }

    public class LinearKeyFrame<T> : KeyFrame<T>
    {
        protected override Freezable CreateInstanceCore() => new LinearKeyFrame<T>();
        protected override T InterpolateValueCore(T baseValue, double keyFrameProgress) => ValueInterpolationHelper<T>.Default.Interpolate(baseValue, Value, keyFrameProgress);
    }

    public class DiscreteKeyFrame<T> : KeyFrame<T>
    {
        protected override Freezable CreateInstanceCore() => new DiscreteKeyFrame<T>();
        protected override T InterpolateValueCore(T baseValue, double keyFrameProgress) => baseValue;
    }

    public abstract class KeyFrame<T> : Freezable, IKeyFrame
    {
        public KeyTime KeyTime { get => (KeyTime)GetValue(KeyTimeProperty); set => SetValue(KeyTimeProperty, value); }
        public static readonly DependencyProperty KeyTimeProperty = DependencyProperty.Register("KeyTime", typeof(KeyTime), typeof(KeyFrame<T>), new PropertyMetadata(KeyTime.Uniform));

        object IKeyFrame.Value { get => GetValue(ValueProperty); set => SetValue(ValueProperty, value); }
        public T Value { get => (T)GetValue(ValueProperty); set => SetValue(ValueProperty, value); }
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(T), typeof(KeyFrame<T>));

        public T InterpolateValue(T baseValue, double keyFrameProgress)
        {
            if (keyFrameProgress < 0.0 || keyFrameProgress > 1.0) throw new ArgumentOutOfRangeException("keyFrameProgress");
            return keyFrameProgress switch
            {
                0.0 => baseValue,
                1.0 => Value,
                _ => InterpolateValueCore(baseValue, keyFrameProgress)
            };
        }

        protected abstract T InterpolateValueCore(T baseValue, double keyFrameProgress);
    }

    public class EasingAbsoluteKeyFrame<T> : AbsoluteKeyFrame<T>
    {
        public IEasingFunction EasingFunction { get => (IEasingFunction)GetValue(EasingFunctionProperty); set => SetValue(EasingFunctionProperty, value); }
        public static readonly DependencyProperty EasingFunctionProperty = DependencyProperty.Register("EasingFunction", typeof(IEasingFunction), typeof(EasingKeyFrame<T>));

        protected override Freezable CreateInstanceCore() => new EasingAbsoluteKeyFrame<T>();
        public override double EaseProgress(double keyFrameProgress) => EasingFunction?.Ease(keyFrameProgress) ?? keyFrameProgress;
    }

    public class SplineAbsoluteKeyFrame<T> : AbsoluteKeyFrame<T>
    {
        public KeySpline KeySpline { get => (KeySpline)GetValue(KeySplineProperty); set => SetValue(KeySplineProperty, value); }
        public static readonly DependencyProperty KeySplineProperty = DependencyProperty.Register("KeySpline", typeof(KeySpline), typeof(SplineKeyFrame<T>), new PropertyMetadata(new KeySpline()), v => v != null);

        protected override Freezable CreateInstanceCore() => new SplineAbsoluteKeyFrame<T>();
        public override double EaseProgress(double keyFrameProgress) => KeySpline?.GetSplineProgress(keyFrameProgress) ?? keyFrameProgress;
    }

    public class LinearAbsoluteKeyFrame<T> : AbsoluteKeyFrame<T>
    {
        protected override Freezable CreateInstanceCore() => new LinearAbsoluteKeyFrame<T>();
    }

    public class DiscreteAbsoluteKeyFrame<T> : AbsoluteKeyFrame<T>
    {
        protected override Freezable CreateInstanceCore() => new DiscreteAbsoluteKeyFrame<T>();
        public override double EaseProgress(double keyFrameProgress) => Math.Truncate(keyFrameProgress);
    }

    public interface IAbsoluteKeyFrame
    {
        long FramesCount { get; set; }
        object Value { get; set; }
    }

    public abstract class AbsoluteKeyFrame<T> : Freezable, IAbsoluteKeyFrame
    {
        public const long FPS = 60;

        public long FramesCount { get => (long)GetValue(FramesCountProperty); set => SetValue(FramesCountProperty, value); }
        public static readonly DependencyProperty FramesCountProperty = DependencyProperty.Register("FramesCount", typeof(long), typeof(AbsoluteKeyFrame<T>));
        public event PropertyChangedExtendedEventHandler<long> FramesCountChanged;

        object IAbsoluteKeyFrame.Value { get => GetValue(ValueProperty); set => SetValue(ValueProperty, value); }
        public T Value { get => (T)GetValue(ValueProperty); set => SetValue(ValueProperty, value); }
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(T), typeof(AbsoluteKeyFrame<T>));
        public event PropertyChangedExtendedEventHandler<T> ValueChanged;

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.Property == FramesCountProperty) FramesCountChanged?.Invoke(this, new PropertyChangedExtendedEventArgs<long>("FramesCount", (long)e.OldValue, (long)e.NewValue));
            else if (e.Property == ValueProperty) ValueChanged?.Invoke(this, new PropertyChangedExtendedEventArgs<T>("Value", (T)e.OldValue, (T)e.NewValue));
            base.OnPropertyChanged(e);
        }

        public T InterpolateValue(T baseValue, double keyFrameProgress)
        {
            if (keyFrameProgress < 0.0 || keyFrameProgress > 1.0) throw new ArgumentOutOfRangeException("keyFrameProgress");
            return keyFrameProgress switch
            {
                0.0 => baseValue,
                1.0 => Value,
                _ => InterpolateValueCore(baseValue, keyFrameProgress)
            };
        }

        public virtual double EaseProgress(double keyFrameProgress) => keyFrameProgress;
        protected virtual T InterpolateValueCore(T baseValue, double keyFrameProgress) => ValueInterpolationHelper<T>.Default.Interpolate(baseValue, Value, EaseProgress(keyFrameProgress));
    }

    public class KeySpline : Freezable
    {
        private (double cx, double cy)[] m_tcoefs = new[] { (-2.0, -2.0), (3.0, 3.0), (0.0, 0.0), (0.0, 0.0) };

        public Point ControlPoint1 { get => (Point)GetValue(ControlPoint1Property); set => SetValue(ControlPoint1Property, value); }
        public static readonly DependencyProperty ControlPoint1Property = DependencyProperty.Register("ControlPoint1", typeof(Point), typeof(KeySpline), new PropertyMetadata(new Point(0, 0)));

        public Point ControlPoint2 { get => (Point)GetValue(ControlPoint2Property); set => SetValue(ControlPoint2Property, value); }
        public static readonly DependencyProperty ControlPoint2Property = DependencyProperty.Register("ControlPoint2", typeof(Point), typeof(KeySpline), new PropertyMetadata(new Point(1, 1)));

        public double GetSplineProgress(double linearProgress)
        {
            var tcoefs = m_tcoefs;
            var (a, b, c, d) = (tcoefs[0].cx, tcoefs[1].cx, tcoefs[2].cx, tcoefs[3].cx);
            var (t0, t1, t2) = MathNet.Numerics.RootFinding.Cubic.RealRoots((d - linearProgress) / a, c / a, b / a);
            double t = rootValid(t0) ? t0 : rootValid(t1) ? t1 : rootValid(t2) ? t2 : double.NaN;
            return Num.GetBezierPointFromTCoefs(t, tcoefs).y;
            static bool rootValid(double x) => 0 <= x && x <= 1;
        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.Property == ControlPoint1Property || e.Property == ControlPoint2Property)
            {
                var cp1 = ControlPoint1;
                var cp2 = ControlPoint2;
                m_tcoefs = Num.GetTCoefs((0, 0), (cp1.X, cp1.Y), (cp2.X, cp2.Y), (1, 1));
            }
            base.OnPropertyChanged(e);
        }

        protected override Freezable CreateInstanceCore() => new KeySpline();
    }

    public interface IAbsoluteKeyFrameCollection : IList, INotifyCollectionChanged
    {
        event EventHandler Changed;
        int IndexOfKeyFrameAt(long framesCount);
        double ProgressAt(long framesCount, bool ease);
        double ProgressAt(int index, double framesCount, bool ease);
        object ValueAt(long framesCount);
        void RemoveKeyFrameAt(long framesCount);
        void PutKeyFrame(IAbsoluteKeyFrame keyFrame);
    }

    public class AbsoluteKeyFrameCollection<T> : ObservableCollection<AbsoluteKeyFrame<T>>, IAbsoluteKeyFrameCollection
    {
        public event EventHandler Changed;

        private int GetNewIndex(AbsoluteKeyFrame<T> item)
        {
            long framesCount = item.FramesCount;
            int i = Items.IndexOf(kf => kf.FramesCount >= framesCount);
            return i == -1 ? Count : i;
        }

        private void UpdateKeyFramePosition(AbsoluteKeyFrame<T> item) => UpdateKeyFramePosition(IndexOf(item), item);
        private void UpdateKeyFramePosition(int index, AbsoluteKeyFrame<T> item)
        {
            if (index > 0 && item.FramesCount < Items[index - 1].FramesCount) Move(index, index - 1);
            else if (index < Count - 1 && item.FramesCount > Items[index + 1].FramesCount) Move(index, index + 1);
        }

        private void Register(AbsoluteKeyFrame<T> item)
        {
            item.FramesCountChanged += Item_FramesCountChanged;
            item.Changed += Item_Changed;
        }
        private void UnRegister(AbsoluteKeyFrame<T> item)
        {
            item.FramesCountChanged -= Item_FramesCountChanged;
            item.Changed -= Item_Changed;
        }

        private void Item_Changed(object sender, EventArgs e) => Changed?.Invoke(this, EventArgs.Empty);

        public int IndexOfKeyFrameAt(long framesCount)
        {
            int i = Items.IndexOf(kf => kf.FramesCount >= framesCount);
            return i == -1 ? Count : i;
        }

        public double ProgressAt(long framesCount, bool ease) => ProgressAt(IndexOfKeyFrameAt(framesCount), framesCount, ease);
        public double ProgressAt(int index, double framesCount, bool ease)
        {
            if (index == 0) return 1;
            if (index == Count) return 0;
            var currentKeyFrame = Items[index];
            long previousFramesCount = Items[index - 1].FramesCount;
            double linearProgress = (framesCount - previousFramesCount) / (currentKeyFrame.FramesCount - previousFramesCount);
            return ease ? currentKeyFrame.EaseProgress(linearProgress) : linearProgress;
        }
        private double ProgressAt(int index, long framesCount, bool ease)
        {
            if (index == 0) return 1;
            if (index == Count) return 0;
            var currentKeyFrame = Items[index];
            long previousFramesCount = Items[index - 1].FramesCount;
            double linearProgress = (double)(framesCount - previousFramesCount) / (currentKeyFrame.FramesCount - previousFramesCount);
            return ease ? currentKeyFrame.EaseProgress(linearProgress) : linearProgress;
        }

        object IAbsoluteKeyFrameCollection.ValueAt(long framesCount) => ValueAt(framesCount);
        public T ValueAt(long framesCount)
        {
            int i = IndexOfKeyFrameAt(framesCount);
            return
                i == 0 ? Items.First().Value :
                i == Count ? Items.Last().Value :
                Items[i].InterpolateValue(Items[i - 1].Value, ProgressAt(i, framesCount, false));
        }

        public void RemoveKeyFrameAt(long framesCount) => PutKeyFrame(framesCount, null);

        void IAbsoluteKeyFrameCollection.PutKeyFrame(IAbsoluteKeyFrame keyFrame) { if (keyFrame is AbsoluteKeyFrame<T> absoluteKeyFrame) PutKeyFrame(absoluteKeyFrame); }
        public void PutKeyFrame(AbsoluteKeyFrame<T> keyFrame) { if (keyFrame != null) PutKeyFrame(keyFrame.FramesCount, keyFrame); }
        private void PutKeyFrame(long framesCount, AbsoluteKeyFrame<T> keyFrame)
        {
            int index = Items.IndexOf(kf => kf.FramesCount == framesCount);
            if (index == -1) Add(keyFrame);
            else this[index] = keyFrame;
        }

        private void Item_FramesCountChanged(object sender, PropertyChangedExtendedEventArgs<long> e) => UpdateKeyFramePosition((AbsoluteKeyFrame<T>)sender);

        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            base.OnCollectionChanged(e);
            Changed?.Invoke(this, EventArgs.Empty);
        }

        protected override void ClearItems()
        {
            foreach (var item in Items) UnRegister(item);
            base.ClearItems();
        }
        protected override void InsertItem(int index, AbsoluteKeyFrame<T> item)
        {
            if (item != null)
            {
                Register(item);
                base.InsertItem(GetNewIndex(item), item);
            }
        }
        protected override void RemoveItem(int index)
        {
            UnRegister(Items[index]);
            base.RemoveItem(index);
        }
        protected override void SetItem(int index, AbsoluteKeyFrame<T> item)
        {
            if (item == null) RemoveAt(index);
            else
            {
                UnRegister(Items[index]);
                Register(item);
                base.SetItem(index, item);
                UpdateKeyFramePosition(index, item);
            }
        }
    }

    public static class Animating
    {
        public static Dictionary<string, StaticAnimation> Animations { get; } = new Dictionary<string, StaticAnimation>();

        #region DoubleAnimation

        public static async Task Animate<T>(string name, Action<T> setter, T from, T to, TimeSpan duration, RepeatBehavior repeatBehavior = default, bool autoReverse = false, bool isCumulative = false, IEasingFunction easingFunction = null, int? fps = null)
        {
            bool finished = false;
            var tcs = new TaskCompletionSource<object>();
            var animation = new StaticAnimation();
            long iterationsCount = 0;

            double durationMilliseconds = duration.TotalMilliseconds;

            Stopwatch stopwatch = null;
            FrameStopwatch frameStopwatch = null;
            if (fps.HasValue) frameStopwatch = new FrameStopwatch(fps.Value, 0);
            else stopwatch = new Stopwatch();

            double CurrentMilliseconds() => fps.HasValue ? frameStopwatch.ElapsedMilliseconds : stopwatch.ElapsedMilliseconds;

            TimeSpan totalDuration;
            if (repeatBehavior.HasCount) totalDuration = repeatBehavior.Count == 0 ? duration : duration.Multiply(repeatBehavior.Count);
            else if (repeatBehavior.HasDuration) totalDuration = repeatBehavior.Duration;
            else totalDuration = TimeSpan.MaxValue;

            double totalMilliseconds = totalDuration.TotalMilliseconds;

            if (name != null) animation.StateChanged += StateChanged;

            void StateChanged(object sender, EventArgs<StaticAnimationState> e)
            {
                switch (e.Param1)
                {
                    case StaticAnimationState.Running:
                        stopwatch?.Start();
                        CompositionTarget.Rendering += Animate;
                        break;
                    case StaticAnimationState.Paused:
                        stopwatch?.Stop();
                        CompositionTarget.Rendering -= Animate;
                        break;
                    case StaticAnimationState.Stopped:
                        Finish();
                        break;
                }
            }

            void Animate(object sender, EventArgs e)
            {
                double baseCurrentMilliseconds = CurrentMilliseconds();
                double currentMilliseconds = baseCurrentMilliseconds;
                if (currentMilliseconds > (iterationsCount + 1) * durationMilliseconds && currentMilliseconds < totalMilliseconds) iterationsCount++;

                currentMilliseconds -= durationMilliseconds * iterationsCount;
                if (autoReverse && iterationsCount % 2 != 0) currentMilliseconds = -currentMilliseconds + durationMilliseconds;

                double baseProgress = Math.Min(currentMilliseconds / durationMilliseconds, 1);
                double progress = easingFunction?.Ease(baseProgress) ?? baseProgress;

                setter(ValueInterpolationHelper<T>.Default.Interpolate(from, to, isCumulative ? iterationsCount + progress : progress));

                frameStopwatch?.Spend();

                if (baseCurrentMilliseconds >= totalMilliseconds) Finish();
            }

            void Finish()
            {
                if (!finished)
                {
                    finished = true;
                    stopwatch?.Stop();
                    CompositionTarget.Rendering -= Animate;
                    if (name != null)
                    {
                        animation.StateChanged -= StateChanged;
                        Animations.Remove(name);
                        animation.End();
                    }
                    tcs.TrySetResult(null);
                }
            }

            CompositionTarget.Rendering += Animate;

            if (name != null)
            {
                if (Animations.TryGetValue(name, out var oldAnimation))
                {
                    oldAnimation.Stop();
                    await oldAnimation.Wait();
                }
                Animations.Add(name, animation);
            }

            stopwatch?.Start();
            await tcs.Task;
        }

        public static async Task AnimateArray<T>(string name, Action<T[]> setter, IList<T> from, IList<T> to, TimeSpan duration, RepeatBehavior repeatBehavior = default, bool autoReverse = false, bool isCumulative = false, IEasingFunction easingFunction = null, int? fps = null)
        {
            if (from.Count != to.Count) return;

            bool finished = false;
            var tcs = new TaskCompletionSource<object>();
            var animation = new StaticAnimation();
            long iterationsCount = 0;

            double durationMilliseconds = duration.TotalMilliseconds;

            Stopwatch stopwatch = null;
            FrameStopwatch frameStopwatch = null;
            if (fps.HasValue) frameStopwatch = new FrameStopwatch(fps.Value, 0);
            else stopwatch = new Stopwatch();

            double CurrentMilliseconds() => fps.HasValue ? frameStopwatch.ElapsedMilliseconds : stopwatch.ElapsedMilliseconds;

            TimeSpan totalDuration;
            if (repeatBehavior.HasCount) totalDuration = repeatBehavior.Count == 0 ? duration : duration.Multiply(repeatBehavior.Count);
            else if (repeatBehavior.HasDuration) totalDuration = repeatBehavior.Duration;
            else totalDuration = TimeSpan.MaxValue;

            double totalMilliseconds = totalDuration.TotalMilliseconds;

            int count = from.Count;

            if (name != null) animation.StateChanged += StateChanged;

            void StateChanged(object sender, EventArgs<StaticAnimationState> e)
            {
                switch (e.Param1)
                {
                    case StaticAnimationState.Running:
                        stopwatch?.Start();
                        CompositionTarget.Rendering += Animate;
                        break;
                    case StaticAnimationState.Paused:
                        stopwatch?.Stop();
                        CompositionTarget.Rendering -= Animate;
                        break;
                    case StaticAnimationState.Stopped:
                        Finish();
                        break;
                }
            }

            void Animate(object sender, EventArgs e)
            {
                double baseCurrentMilliseconds = CurrentMilliseconds();
                double currentMilliseconds = baseCurrentMilliseconds;
                if (currentMilliseconds > (iterationsCount + 1) * durationMilliseconds && currentMilliseconds < totalMilliseconds) iterationsCount++;

                currentMilliseconds -= durationMilliseconds * iterationsCount;
                if (autoReverse && iterationsCount % 2 != 0) currentMilliseconds = -currentMilliseconds + durationMilliseconds;

                double baseProgress = Math.Min(currentMilliseconds / durationMilliseconds, 1);
                double progress = easingFunction?.Ease(baseProgress) ?? baseProgress;

                var value = new T[count];
                for (int i = 0; i < count; i++) value[i] = ValueInterpolationHelper<T>.Default.Interpolate(from[i], to[i], isCumulative ? progress + iterationsCount : progress);

                setter(value);

                frameStopwatch?.Spend();

                if (baseCurrentMilliseconds >= totalMilliseconds) Finish();
            }

            void Finish()
            {
                if (!finished)
                {
                    finished = true;
                    stopwatch?.Stop();
                    CompositionTarget.Rendering -= Animate;
                    if (name != null)
                    {
                        animation.StateChanged -= StateChanged;
                        Animations.Remove(name);
                        animation.End();
                    }
                    tcs.TrySetResult(null);
                }
            }

            CompositionTarget.Rendering += Animate;

            if (name != null)
            {
                if (Animations.TryGetValue(name, out var oldAnimation))
                {
                    oldAnimation.Stop();
                    await oldAnimation.Wait();
                }
                Animations.Add(name, animation);
            }

            stopwatch?.Start();
            await tcs.Task;
        }

        public static async Task AnimateUsingKeyFrames<T>(string name, Action<T> setter, T from, IEnumerable<KeyFrame<T>> keyFrames, Duration duration, RepeatBehavior repeatBehavior = default, bool autoReverse = false, bool isCumulative = false, int? fps = null)
        {
            bool finished = false;
            var tcs = new TaskCompletionSource<object>();
            var animation = new StaticAnimation();
            long iterationsCount = 0;

            Stopwatch stopwatch = null;
            FrameStopwatch frameStopwatch = null;
            if (fps.HasValue) frameStopwatch = new FrameStopwatch(fps.Value, 0);
            else stopwatch = new Stopwatch();

            TimeSpan CurrentTime() => fps.HasValue ? frameStopwatch.Elapsed : stopwatch.Elapsed;

            var resolvedKeyFrames = ResolveKeyTimes(keyFrames, duration, out var durationTime);

            var totalDuration = repeatBehavior.HasCount ? repeatBehavior.Count == 0 ? durationTime : durationTime.Multiply(repeatBehavior.Count) :
                repeatBehavior.HasDuration ? repeatBehavior.Duration : TimeSpan.MaxValue;

            if (name != null) animation.StateChanged += StateChanged;

            void StateChanged(object sender, EventArgs<StaticAnimationState> e)
            {
                switch (e.Param1)
                {
                    case StaticAnimationState.Running:
                        stopwatch?.Start();
                        CompositionTarget.Rendering += Animate;
                        break;
                    case StaticAnimationState.Paused:
                        stopwatch?.Stop();
                        CompositionTarget.Rendering -= Animate;
                        break;
                    case StaticAnimationState.Stopped:
                        Finish();
                        break;
                }
            }

            void Animate(object sender, EventArgs e)
            {
                var currentTime = CurrentTime();
                if (currentTime > durationTime.Multiply(iterationsCount + 1) && currentTime < totalDuration) iterationsCount++;

                currentTime -= durationTime.Multiply(iterationsCount);
                if (autoReverse && iterationsCount % 2 != 0) currentTime = -currentTime + durationTime;

                int maxKeyFrameIndex = resolvedKeyFrames.Length - 1;
                int currentKeyFrameIndex = 0;
                T currentKeyFrameValue;

                while (currentKeyFrameIndex < resolvedKeyFrames.Length && currentTime > resolvedKeyFrames[currentKeyFrameIndex].KeyTime.TimeSpan) currentKeyFrameIndex++;
                while (currentKeyFrameIndex < maxKeyFrameIndex && currentTime == resolvedKeyFrames[currentKeyFrameIndex + 1].KeyTime.TimeSpan) currentKeyFrameIndex++;

                if (currentKeyFrameIndex == resolvedKeyFrames.Length) currentKeyFrameValue = resolvedKeyFrames[maxKeyFrameIndex].Value;
                else if (currentTime == resolvedKeyFrames[currentKeyFrameIndex].KeyTime.TimeSpan) currentKeyFrameValue = resolvedKeyFrames[currentKeyFrameIndex].Value;
                else
                {
                    T fromValue;
                    double currentSegmentProgress;

                    if (currentKeyFrameIndex == 0)
                    {
                        fromValue = from;
                        currentSegmentProgress = currentTime.TotalMilliseconds / resolvedKeyFrames[0].KeyTime.TimeSpan.TotalMilliseconds;
                    }
                    else
                    {
                        int previousKeyFrameIndex = currentKeyFrameIndex - 1;
                        var previousKeyTime = resolvedKeyFrames[previousKeyFrameIndex].KeyTime.TimeSpan;

                        fromValue = resolvedKeyFrames[previousKeyFrameIndex].Value;

                        var segmentCurrentTime = currentTime - previousKeyTime;
                        var segmentDuration = resolvedKeyFrames[currentKeyFrameIndex].KeyTime.TimeSpan - previousKeyTime;

                        currentSegmentProgress = segmentCurrentTime.TotalMilliseconds / segmentDuration.TotalMilliseconds;
                    }

                    currentKeyFrameValue = resolvedKeyFrames[currentKeyFrameIndex].InterpolateValue(fromValue, Math.Min(currentSegmentProgress, 1));
                }

                if (isCumulative) currentKeyFrameValue = ValueInterpolationHelper<T>.Default.Iterate(currentKeyFrameValue, resolvedKeyFrames[0].Value, resolvedKeyFrames.Last().Value, iterationsCount);

                setter(currentKeyFrameValue);

                frameStopwatch?.Spend();

                if (currentTime >= totalDuration) Finish();
            }

            void Finish()
            {
                if (!finished)
                {
                    finished = true;
                    stopwatch?.Stop();
                    CompositionTarget.Rendering -= Animate;
                    if (name != null)
                    {
                        animation.StateChanged -= StateChanged;
                        animation.End();
                        Animations.Remove(name);
                    }
                    tcs.TrySetResult(null);
                }
            }

            CompositionTarget.Rendering += Animate;

            if (name != null)
            {
                if (Animations.TryGetValue(name, out var oldAnimation))
                {
                    oldAnimation.Stop();
                    await oldAnimation.Wait();
                }
                Animations.Add(name, animation);
            }

            stopwatch?.Start();
            await tcs.Task;
        }

        /*public static T GetValue<T>(FrameStopwatch current, KeyFrame<T>[] resolvedKeyFrames) => GetValue(current, resolvedKeyFrames[0].Value, resolvedKeyFrames);
        public static T GetValue<T>(FrameStopwatch current, T from, KeyFrame<T>[] resolvedKeyFrames)
        {
            var currentTime = current.Elapsed;
            int maxKeyFrameIndex = resolvedKeyFrames.Length - 1;
            int currentKeyFrameIndex = 0;
            T currentKeyFrameValue;

            while (currentKeyFrameIndex < resolvedKeyFrames.Length && currentTime > resolvedKeyFrames[currentKeyFrameIndex].KeyTime.TimeSpan) currentKeyFrameIndex++;
            while (currentKeyFrameIndex < maxKeyFrameIndex && currentTime == resolvedKeyFrames[currentKeyFrameIndex + 1].KeyTime.TimeSpan) currentKeyFrameIndex++;

            if (currentKeyFrameIndex == resolvedKeyFrames.Length) currentKeyFrameValue = resolvedKeyFrames[maxKeyFrameIndex].Value;
            else if (currentTime == resolvedKeyFrames[currentKeyFrameIndex].KeyTime.TimeSpan) currentKeyFrameValue = resolvedKeyFrames[currentKeyFrameIndex].Value;
            else
            {
                T fromValue;
                double currentSegmentProgress;

                if (currentKeyFrameIndex == 0)
                {
                    fromValue = from;
                    currentSegmentProgress = currentTime.TotalMilliseconds / resolvedKeyFrames[0].KeyTime.TimeSpan.TotalMilliseconds;
                }
                else
                {
                    int previousKeyFrameIndex = currentKeyFrameIndex - 1;
                    var previousKeyTime = resolvedKeyFrames[previousKeyFrameIndex].KeyTime.TimeSpan;

                    fromValue = resolvedKeyFrames[previousKeyFrameIndex].Value;

                    var segmentCurrentTime = currentTime - previousKeyTime;
                    var segmentDuration = resolvedKeyFrames[currentKeyFrameIndex].KeyTime.TimeSpan - previousKeyTime;

                    currentSegmentProgress = segmentCurrentTime.TotalMilliseconds / segmentDuration.TotalMilliseconds;
                }

                currentKeyFrameValue = resolvedKeyFrames[currentKeyFrameIndex].InterpolateValue(fromValue, Math.Min(currentSegmentProgress, 1));
            }

            return currentKeyFrameValue;
        }*/

        #endregion

        #region PointAnimation

        public static async Task AnimatePoint(string name, Action<Point> setter, Point from, Point to, TimeSpan duration, RepeatBehavior repeatBehavior = default, bool autoReverse = false, bool isCumulative = false, IEasingFunction easingFunction = null, int? fps = null)
        {
            bool finished = false;
            var tcs = new TaskCompletionSource<object>();
            var animation = new StaticAnimation();
            long iterationsCount = 0;

            double durationMilliseconds = duration.TotalMilliseconds;

            Stopwatch stopwatch = null;
            FrameStopwatch frameStopwatch = null;
            if (fps.HasValue) frameStopwatch = new FrameStopwatch(fps.Value, 0);
            else stopwatch = new Stopwatch();

            double CurrentMilliseconds() => fps.HasValue ? frameStopwatch.ElapsedMilliseconds : stopwatch.ElapsedMilliseconds;

            TimeSpan totalDuration;
            if (repeatBehavior.HasCount) totalDuration = repeatBehavior.Count == 0 ? duration : duration.Multiply(repeatBehavior.Count);
            else if (repeatBehavior.HasDuration) totalDuration = repeatBehavior.Duration;
            else totalDuration = TimeSpan.MaxValue;

            double totalMilliseconds = totalDuration.TotalMilliseconds;

            //=============================================================================================
            var diff = to - from;
            //=============================================================================================

            if (name != null) animation.StateChanged += StateChanged;

            void StateChanged(object sender, EventArgs<StaticAnimationState> e)
            {
                switch (e.Param1)
                {
                    case StaticAnimationState.Running:
                        stopwatch?.Start();
                        CompositionTarget.Rendering += Animate;
                        break;
                    case StaticAnimationState.Paused:
                        stopwatch?.Stop();
                        CompositionTarget.Rendering -= Animate;
                        break;
                    case StaticAnimationState.Stopped:
                        Finish();
                        break;
                }
            }

            void Animate(object sender, EventArgs e)
            {
                double baseCurrentMilliseconds = CurrentMilliseconds();
                double currentMilliseconds = baseCurrentMilliseconds;
                if (currentMilliseconds > (iterationsCount + 1) * durationMilliseconds && currentMilliseconds < totalMilliseconds) iterationsCount++;

                currentMilliseconds -= durationMilliseconds * iterationsCount;
                if (autoReverse && iterationsCount % 2 != 0) currentMilliseconds = -currentMilliseconds + durationMilliseconds;

                double baseProgress = Math.Min(currentMilliseconds / durationMilliseconds, 1);
                double progress = easingFunction?.Ease(baseProgress) ?? baseProgress;

                //=============================================================================================
                var value = from + diff * progress;
                if (isCumulative) value += diff * iterationsCount;

                setter(value);
                //=============================================================================================

                frameStopwatch?.Spend();

                if (baseCurrentMilliseconds >= totalMilliseconds) Finish();
            }

            void Finish()
            {
                if (!finished)
                {
                    finished = true;
                    stopwatch?.Stop();
                    CompositionTarget.Rendering -= Animate;
                    if (name != null)
                    {
                        animation.StateChanged -= StateChanged;
                        Animations.Remove(name);
                        animation.End();
                    }
                    tcs.TrySetResult(null);
                }
            }

            CompositionTarget.Rendering += Animate;

            if (name != null)
            {
                if (Animations.TryGetValue(name, out var oldAnimation))
                {
                    oldAnimation.Stop();
                    await oldAnimation.Wait();
                }
                Animations.Add(name, animation);
            }

            stopwatch?.Start();
            await tcs.Task;
        }

        public static async Task AnimatePointArray(string name, Action<Point[]> setter, IList<Point> from, IList<Point> to, TimeSpan duration, RepeatBehavior repeatBehavior = default, bool autoReverse = false, bool isCumulative = false, IEasingFunction easingFunction = null, int? fps = null)
        {
            bool finished = false;
            var tcs = new TaskCompletionSource<object>();
            var animation = new StaticAnimation();
            long iterationsCount = 0;

            double durationMilliseconds = duration.TotalMilliseconds;

            Stopwatch stopwatch = null;
            FrameStopwatch frameStopwatch = null;
            if (fps.HasValue) frameStopwatch = new FrameStopwatch(fps.Value, 0);
            else stopwatch = new Stopwatch();

            double CurrentMilliseconds() => fps.HasValue ? frameStopwatch.ElapsedMilliseconds : stopwatch.ElapsedMilliseconds;

            TimeSpan totalDuration;
            if (repeatBehavior.HasCount) totalDuration = repeatBehavior.Count == 0 ? duration : duration.Multiply(repeatBehavior.Count);
            else if (repeatBehavior.HasDuration) totalDuration = repeatBehavior.Duration;
            else totalDuration = TimeSpan.MaxValue;

            double totalMilliseconds = totalDuration.TotalMilliseconds;

            //=============================================================================================
            int count = from.Count;
            var diff = new Vector[count];

            for (int i = 0; i < count; i++) diff[i] = to[i] - from[i];
            //=============================================================================================

            if (name != null) animation.StateChanged += StateChanged;

            void StateChanged(object sender, EventArgs<StaticAnimationState> e)
            {
                switch (e.Param1)
                {
                    case StaticAnimationState.Running:
                        stopwatch?.Start();
                        CompositionTarget.Rendering += Animate;
                        break;
                    case StaticAnimationState.Paused:
                        stopwatch?.Stop();
                        CompositionTarget.Rendering -= Animate;
                        break;
                    case StaticAnimationState.Stopped:
                        Finish();
                        break;
                }
            }

            void Animate(object sender, EventArgs e)
            {
                double baseCurrentMilliseconds = CurrentMilliseconds();
                double currentMilliseconds = baseCurrentMilliseconds;
                if (currentMilliseconds > (iterationsCount + 1) * durationMilliseconds && currentMilliseconds < totalMilliseconds) iterationsCount++;

                currentMilliseconds -= durationMilliseconds * iterationsCount;
                if (autoReverse && iterationsCount % 2 != 0) currentMilliseconds = -currentMilliseconds + durationMilliseconds;

                double baseProgress = Math.Min(currentMilliseconds / durationMilliseconds, 1);
                double progress = easingFunction?.Ease(baseProgress) ?? baseProgress;

                //=============================================================================================
                var value = new Point[count];
                for (int i = 0; i < count; i++)
                {
                    var dif = diff[i];
                    var val = from[i] + dif * progress;
                    if (isCumulative) val += dif * iterationsCount;

                    value[i] = val;
                }

                setter(value);
                //=============================================================================================

                frameStopwatch?.Spend();

                if (baseCurrentMilliseconds >= totalMilliseconds) Finish();
            }

            void Finish()
            {
                if (!finished)
                {
                    finished = true;
                    stopwatch?.Stop();
                    CompositionTarget.Rendering -= Animate;
                    if (name != null)
                    {
                        animation.StateChanged -= StateChanged;
                        Animations.Remove(name);
                        animation.End();
                    }
                    tcs.TrySetResult(null);
                }
            }

            CompositionTarget.Rendering += Animate;

            if (name != null)
            {
                if (Animations.TryGetValue(name, out var oldAnimation))
                {
                    oldAnimation.Stop();
                    await oldAnimation.Wait();
                }
                Animations.Add(name, animation);
            }

            stopwatch?.Start();
            await tcs.Task;
        }

        public static async Task AnimatePointUsingKeyFrames(string name, Action<Point> setter, Point from, IEnumerable<PointKeyFrame> keyFrames, Duration duration, RepeatBehavior repeatBehavior = default, bool autoReverse = false, bool isCumulative = false, int? fps = null)
        {
            bool finished = false;
            var tcs = new TaskCompletionSource<object>();
            var animation = new StaticAnimation();
            long iterationsCount = 0;

            Stopwatch stopwatch = null;
            FrameStopwatch frameStopwatch = null;
            if (fps.HasValue) frameStopwatch = new FrameStopwatch(fps.Value, 0);
            else stopwatch = new Stopwatch();

            TimeSpan CurrentTime() => fps.HasValue ? frameStopwatch.Elapsed : stopwatch.Elapsed;

            var resolvedKeyFrames = ResolveKeyTimes(keyFrames, duration, out var durationTime);

            //=============================================================================================
            var diff = resolvedKeyFrames[resolvedKeyFrames.Length - 1].Value - resolvedKeyFrames[0].Value;
            //=============================================================================================

            TimeSpan totalDuration;
            if (repeatBehavior.HasCount) totalDuration = repeatBehavior.Count == 0 ? durationTime : durationTime.Multiply(repeatBehavior.Count);
            else if (repeatBehavior.HasDuration) totalDuration = repeatBehavior.Duration;
            else totalDuration = TimeSpan.MaxValue;

            if (name != null) animation.StateChanged += StateChanged;

            void StateChanged(object sender, EventArgs<StaticAnimationState> e)
            {
                switch (e.Param1)
                {
                    case StaticAnimationState.Running:
                        stopwatch?.Start();
                        CompositionTarget.Rendering += Animate;
                        break;
                    case StaticAnimationState.Paused:
                        stopwatch?.Stop();
                        CompositionTarget.Rendering -= Animate;
                        break;
                    case StaticAnimationState.Stopped:
                        Finish();
                        break;
                }
            }

            void Animate(object sender, EventArgs e)
            {
                var currentTime = CurrentTime();
                if (currentTime > durationTime.Multiply(iterationsCount + 1) && currentTime < totalDuration) iterationsCount++;

                currentTime -= durationTime.Multiply(iterationsCount);
                if (autoReverse && iterationsCount % 2 != 0) currentTime = -currentTime + durationTime;

                int maxKeyFrameIndex = resolvedKeyFrames.Length - 1;
                int currentKeyFrameIndex = 0;
                Point currentKeyFrameValue;

                while (currentKeyFrameIndex < resolvedKeyFrames.Length && currentTime > resolvedKeyFrames[currentKeyFrameIndex].KeyTime.TimeSpan) currentKeyFrameIndex++;
                while (currentKeyFrameIndex < maxKeyFrameIndex && currentTime == resolvedKeyFrames[currentKeyFrameIndex + 1].KeyTime.TimeSpan) currentKeyFrameIndex++;

                if (currentKeyFrameIndex == resolvedKeyFrames.Length) currentKeyFrameValue = resolvedKeyFrames[maxKeyFrameIndex].Value;
                else if (currentTime == resolvedKeyFrames[currentKeyFrameIndex].KeyTime.TimeSpan) currentKeyFrameValue = resolvedKeyFrames[currentKeyFrameIndex].Value;
                else
                {
                    Point fromValue;
                    double currentSegmentProgress;

                    if (currentKeyFrameIndex == 0)
                    {
                        fromValue = from;
                        currentSegmentProgress = currentTime.TotalMilliseconds / resolvedKeyFrames[0].KeyTime.TimeSpan.TotalMilliseconds;
                    }
                    else
                    {
                        int previousKeyFrameIndex = currentKeyFrameIndex - 1;
                        var previousKeyTime = resolvedKeyFrames[previousKeyFrameIndex].KeyTime.TimeSpan;

                        fromValue = resolvedKeyFrames[previousKeyFrameIndex].Value;

                        var segmentCurrentTime = currentTime - previousKeyTime;
                        var segmentDuration = resolvedKeyFrames[currentKeyFrameIndex].KeyTime.TimeSpan - previousKeyTime;

                        currentSegmentProgress = segmentCurrentTime.TotalMilliseconds / segmentDuration.TotalMilliseconds;
                    }

                    currentKeyFrameValue = resolvedKeyFrames[currentKeyFrameIndex].InterpolateValue(fromValue, Math.Min(currentSegmentProgress, 1));
                }

                //=============================================================================================
                if (isCumulative) currentKeyFrameValue += diff * iterationsCount;

                setter(currentKeyFrameValue);
                //=============================================================================================

                frameStopwatch?.Spend();

                if (currentTime >= totalDuration) Finish();
            }

            void Finish()
            {
                if (!finished)
                {
                    finished = true;
                    stopwatch?.Stop();
                    CompositionTarget.Rendering -= Animate;
                    if (name != null)
                    {
                        animation.StateChanged -= StateChanged;
                        animation.End();
                        Animations.Remove(name);
                    }
                    tcs.TrySetResult(null);
                }
            }

            CompositionTarget.Rendering += Animate;

            if (name != null)
            {
                if (Animations.TryGetValue(name, out var oldAnimation))
                {
                    oldAnimation.Stop();
                    await oldAnimation.Wait();
                }
                Animations.Add(name, animation);
            }

            stopwatch?.Start();
            await tcs.Task;
        }

        public static async Task AnimatePointAlongPath(string name, Action<Point> setter, double from, double to, PathGeometry pathGeometry, TimeSpan duration, RepeatBehavior repeatBehavior = default, bool autoReverse = false, bool isCumulative = false, IEasingFunction easingFunction = null, int? fps = null)
        {
            bool finished = false;
            var tcs = new TaskCompletionSource<object>();
            var animation = new StaticAnimation();
            long iterationsCount = 0;

            double durationMilliseconds = duration.TotalMilliseconds;

            Stopwatch stopwatch = null;
            FrameStopwatch frameStopwatch = null;
            if (fps.HasValue) frameStopwatch = new FrameStopwatch(fps.Value, 0);
            else stopwatch = new Stopwatch();

            double CurrentMilliseconds() => fps.HasValue ? frameStopwatch.ElapsedMilliseconds : stopwatch.ElapsedMilliseconds;

            TimeSpan totalDuration;
            if (repeatBehavior.HasCount) totalDuration = repeatBehavior.Count == 0 ? duration : duration.Multiply(repeatBehavior.Count);
            else if (repeatBehavior.HasDuration) totalDuration = repeatBehavior.Duration;
            else totalDuration = TimeSpan.MaxValue;

            double totalMilliseconds = totalDuration.TotalMilliseconds;

            //=============================================================================================
            double diff = to - from;
            Vector pointDiff = default;

            pathGeometry.GetPointAtFractionLength(0.0, out var firstPoint, out var firstTangent);
            pathGeometry.GetPointAtFractionLength(1.0, out var lastPoint, out var lastTangent);

            if (isCumulative)
            {
                pointDiff.X = lastPoint.X - firstPoint.X;
                pointDiff.Y = lastPoint.Y - firstPoint.Y;
            }
            //=============================================================================================

            if (name != null) animation.StateChanged += StateChanged;

            void StateChanged(object sender, EventArgs<StaticAnimationState> e)
            {
                switch (e.Param1)
                {
                    case StaticAnimationState.Running:
                        stopwatch?.Start();
                        CompositionTarget.Rendering += Animate;
                        break;
                    case StaticAnimationState.Paused:
                        stopwatch?.Stop();
                        CompositionTarget.Rendering -= Animate;
                        break;
                    case StaticAnimationState.Stopped:
                        Finish();
                        break;
                }
            }

            void Animate(object sender, EventArgs e)
            {
                double baseCurrentMilliseconds = CurrentMilliseconds();
                double currentMilliseconds = baseCurrentMilliseconds;
                if (currentMilliseconds > (iterationsCount + 1) * durationMilliseconds && currentMilliseconds < totalMilliseconds) iterationsCount++;

                currentMilliseconds -= durationMilliseconds * iterationsCount;
                if (autoReverse && iterationsCount % 2 != 0) currentMilliseconds = -currentMilliseconds + durationMilliseconds;

                double baseProgress = Math.Min(currentMilliseconds / durationMilliseconds, 1);
                double progress = easingFunction?.Ease(baseProgress) ?? baseProgress;

                //=============================================================================================
                double value = from + diff * progress;

                pathGeometry.GetPointAtFractionLength(value, out var point, out var tangent);

                if (isCumulative) point += pointDiff * iterationsCount;

                setter(point);
                //=============================================================================================

                frameStopwatch?.Spend();

                if (baseCurrentMilliseconds >= totalMilliseconds) Finish();
            }

            void Finish()
            {
                if (!finished)
                {
                    finished = true;
                    stopwatch?.Stop();
                    CompositionTarget.Rendering -= Animate;
                    if (name != null)
                    {
                        animation.StateChanged -= StateChanged;
                        Animations.Remove(name);
                        animation.End();
                    }
                    tcs.TrySetResult(null);
                }
            }

            CompositionTarget.Rendering += Animate;

            if (name != null)
            {
                if (Animations.TryGetValue(name, out var oldAnimation))
                {
                    oldAnimation.Stop();
                    await oldAnimation.Wait();
                }
                Animations.Add(name, animation);
            }

            stopwatch?.Start();
            await tcs.Task;
        }
        public static Task AnimatePointAlongPath(string name, Action<Point> setter, double from, double to, Geometry geometry, TimeSpan duration, RepeatBehavior repeatBehavior = default, bool autoReverse = false, bool isCumulative = false, IEasingFunction easingFunction = null, int? fps = null) => AnimatePointAlongPath(name, setter, from, to, PathGeometry.CreateFromGeometry(geometry), duration, repeatBehavior, autoReverse, isCumulative, easingFunction, fps);

        public static async Task AnimatePointAlongPathUsingKeyFrames(string name, Action<Point> setter, double from, IEnumerable<DoubleKeyFrame> keyFrames, PathGeometry pathGeometry, Duration duration, RepeatBehavior repeatBehavior = default, bool autoReverse = false, bool isCumulative = false, int? fps = null)
        {
            bool finished = false;
            var tcs = new TaskCompletionSource<object>();
            var animation = new StaticAnimation();
            long iterationsCount = 0;

            Stopwatch stopwatch = null;
            FrameStopwatch frameStopwatch = null;
            if (fps.HasValue) frameStopwatch = new FrameStopwatch(fps.Value, 0);
            else stopwatch = new Stopwatch();

            TimeSpan CurrentTime() => fps.HasValue ? frameStopwatch.Elapsed : stopwatch.Elapsed;

            var resolvedKeyFrames = ResolveKeyTimes(keyFrames, duration, out var durationTime);

            //=============================================================================================
            Vector pointDiff = default;

            pathGeometry.GetPointAtFractionLength(resolvedKeyFrames[0].Value, out var firstPoint, out var firstTangent);
            pathGeometry.GetPointAtFractionLength(resolvedKeyFrames[resolvedKeyFrames.Length - 1].Value, out var lastPoint, out var lastTangent);

            if (isCumulative)
            {
                pointDiff.X = lastPoint.X - firstPoint.X;
                pointDiff.Y = lastPoint.Y - firstPoint.Y;
            }
            //=============================================================================================

            TimeSpan totalDuration;
            if (repeatBehavior.HasCount) totalDuration = repeatBehavior.Count == 0 ? durationTime : durationTime.Multiply(repeatBehavior.Count);
            else if (repeatBehavior.HasDuration) totalDuration = repeatBehavior.Duration;
            else totalDuration = TimeSpan.MaxValue;

            if (name != null) animation.StateChanged += StateChanged;

            void StateChanged(object sender, EventArgs<StaticAnimationState> e)
            {
                switch (e.Param1)
                {
                    case StaticAnimationState.Running:
                        stopwatch?.Start();
                        CompositionTarget.Rendering += Animate;
                        break;
                    case StaticAnimationState.Paused:
                        stopwatch?.Stop();
                        CompositionTarget.Rendering -= Animate;
                        break;
                    case StaticAnimationState.Stopped:
                        Finish();
                        break;
                }
            }

            void Animate(object sender, EventArgs e)
            {
                var currentTime = CurrentTime();
                if (currentTime > durationTime.Multiply(iterationsCount + 1) && currentTime < totalDuration) iterationsCount++;

                currentTime -= durationTime.Multiply(iterationsCount);
                if (autoReverse && iterationsCount % 2 != 0) currentTime = -currentTime + durationTime;

                int maxKeyFrameIndex = resolvedKeyFrames.Length - 1;
                int currentKeyFrameIndex = 0;
                double currentKeyFrameValue;

                while (currentKeyFrameIndex < resolvedKeyFrames.Length && currentTime > resolvedKeyFrames[currentKeyFrameIndex].KeyTime.TimeSpan) currentKeyFrameIndex++;
                while (currentKeyFrameIndex < maxKeyFrameIndex && currentTime == resolvedKeyFrames[currentKeyFrameIndex + 1].KeyTime.TimeSpan) currentKeyFrameIndex++;

                if (currentKeyFrameIndex == resolvedKeyFrames.Length) currentKeyFrameValue = resolvedKeyFrames[maxKeyFrameIndex].Value;
                else if (currentTime == resolvedKeyFrames[currentKeyFrameIndex].KeyTime.TimeSpan) currentKeyFrameValue = resolvedKeyFrames[currentKeyFrameIndex].Value;
                else
                {
                    double fromValue;
                    double currentSegmentProgress;

                    if (currentKeyFrameIndex == 0)
                    {
                        fromValue = from;
                        currentSegmentProgress = currentTime.TotalMilliseconds / resolvedKeyFrames[0].KeyTime.TimeSpan.TotalMilliseconds;
                    }
                    else
                    {
                        int previousKeyFrameIndex = currentKeyFrameIndex - 1;
                        var previousKeyTime = resolvedKeyFrames[previousKeyFrameIndex].KeyTime.TimeSpan;

                        fromValue = resolvedKeyFrames[previousKeyFrameIndex].Value;

                        var segmentCurrentTime = currentTime - previousKeyTime;
                        var segmentDuration = resolvedKeyFrames[currentKeyFrameIndex].KeyTime.TimeSpan - previousKeyTime;

                        currentSegmentProgress = segmentCurrentTime.TotalMilliseconds / segmentDuration.TotalMilliseconds;
                    }

                    currentKeyFrameValue = resolvedKeyFrames[currentKeyFrameIndex].InterpolateValue(fromValue, Math.Min(currentSegmentProgress, 1));
                }

                //=============================================================================================
                pathGeometry.GetPointAtFractionLength(currentKeyFrameValue, out var point, out var tangent);

                if (isCumulative) point += pointDiff * iterationsCount;

                setter(point);
                //=============================================================================================

                frameStopwatch?.Spend();

                if (currentTime >= totalDuration) Finish();
            }

            void Finish()
            {
                if (!finished)
                {
                    finished = true;
                    stopwatch?.Stop();
                    CompositionTarget.Rendering -= Animate;
                    if (name != null)
                    {
                        animation.StateChanged -= StateChanged;
                        animation.End();
                        Animations.Remove(name);
                    }
                    tcs.TrySetResult(null);
                }
            }

            CompositionTarget.Rendering += Animate;

            if (name != null)
            {
                if (Animations.TryGetValue(name, out var oldAnimation))
                {
                    oldAnimation.Stop();
                    await oldAnimation.Wait();
                }
                Animations.Add(name, animation);
            }

            stopwatch?.Start();
            await tcs.Task;
        }
        public static Task AnimatePointAlongPathUsingKeyFrames(string name, Action<Point> setter, double from, IEnumerable<DoubleKeyFrame> keyFrames, Geometry geometry, Duration duration, RepeatBehavior repeatBehavior = default, bool autoReverse = false, bool isCumulative = false, int? fps = null) => AnimatePointAlongPathUsingKeyFrames(name, setter, from, keyFrames, PathGeometry.CreateFromGeometry(geometry), duration, repeatBehavior, autoReverse, isCumulative, fps);

        #endregion

        #region MatrixtAnimation

        public static async Task AnimateMatrixAlongPath(string name, Action<Matrix> setter, double from, double to, PathGeometry pathGeometry, bool doesRotateWithTangent, TimeSpan duration, RepeatBehavior repeatBehavior = default, bool autoReverse = false, bool isOffsetCumulative = false, bool isAngleCumulative = false, IEasingFunction easingFunction = null, int? fps = null)
        {
            bool finished = false;
            var tcs = new TaskCompletionSource<object>();
            var animation = new StaticAnimation();
            long iterationsCount = 0;

            double durationMilliseconds = duration.TotalMilliseconds;

            Stopwatch stopwatch = null;
            FrameStopwatch frameStopwatch = null;
            if (fps.HasValue) frameStopwatch = new FrameStopwatch(fps.Value, 0);
            else stopwatch = new Stopwatch();

            double CurrentMilliseconds() => fps.HasValue ? frameStopwatch.ElapsedMilliseconds : stopwatch.ElapsedMilliseconds;

            TimeSpan totalDuration;
            if (repeatBehavior.HasCount) totalDuration = repeatBehavior.Count == 0 ? duration : duration.Multiply(repeatBehavior.Count);
            else if (repeatBehavior.HasDuration) totalDuration = repeatBehavior.Duration;
            else totalDuration = TimeSpan.MaxValue;

            double totalMilliseconds = totalDuration.TotalMilliseconds;

            //=============================================================================================
            double diff = to - from;
            Vector pointDiff = default;
            double angleDiff = 0.0;

            pathGeometry.GetPointAtFractionLength(0.0, out var firstPoint, out var firstTangent);
            pathGeometry.GetPointAtFractionLength(1.0, out var lastPoint, out var lastTangent);

            if (isOffsetCumulative)
            {
                pointDiff.X = lastPoint.X - firstPoint.X;
                pointDiff.Y = lastPoint.Y - firstPoint.Y;
            }
            if (isAngleCumulative && doesRotateWithTangent) angleDiff = CalculateAngleFromTangentVector(firstTangent.X, firstTangent.Y) - CalculateAngleFromTangentVector(lastTangent.X, lastTangent.Y);
            //=============================================================================================

            if (name != null) animation.StateChanged += StateChanged;

            void StateChanged(object sender, EventArgs<StaticAnimationState> e)
            {
                switch (e.Param1)
                {
                    case StaticAnimationState.Running:
                        stopwatch?.Start();
                        CompositionTarget.Rendering += Animate;
                        break;
                    case StaticAnimationState.Paused:
                        stopwatch?.Stop();
                        CompositionTarget.Rendering -= Animate;
                        break;
                    case StaticAnimationState.Stopped:
                        Finish();
                        break;
                }
            }

            void Animate(object sender, EventArgs e)
            {
                double baseCurrentMilliseconds = CurrentMilliseconds();
                double currentMilliseconds = baseCurrentMilliseconds;
                if (currentMilliseconds > (iterationsCount + 1) * durationMilliseconds && currentMilliseconds < totalMilliseconds) iterationsCount++;

                currentMilliseconds -= durationMilliseconds * iterationsCount;
                if (autoReverse && iterationsCount % 2 != 0) currentMilliseconds = -currentMilliseconds + durationMilliseconds;

                double baseProgress = Math.Min(currentMilliseconds / durationMilliseconds, 1);
                double progress = easingFunction?.Ease(baseProgress) ?? baseProgress;

                //=============================================================================================
                double value = from + diff * progress;

                pathGeometry.GetPointAtFractionLength(value, out var point, out var tangent);
                double angle = doesRotateWithTangent ? CalculateAngleFromTangentVector(tangent.X, tangent.Y) : 0.0;

                var matrix = new Matrix();

                if (isOffsetCumulative) point += pointDiff * iterationsCount;
                if (isAngleCumulative && doesRotateWithTangent) angle += angleDiff * iterationsCount;

                matrix.Rotate(angle);
                matrix.Translate(point.X, point.Y);

                setter(matrix);
                //=============================================================================================

                frameStopwatch?.Spend();

                if (baseCurrentMilliseconds >= totalMilliseconds) Finish();
            }

            void Finish()
            {
                if (!finished)
                {
                    finished = true;
                    stopwatch?.Stop();
                    CompositionTarget.Rendering -= Animate;
                    if (name != null)
                    {
                        animation.StateChanged -= StateChanged;
                        Animations.Remove(name);
                        animation.End();
                    }
                    tcs.TrySetResult(null);
                }
            }

            CompositionTarget.Rendering += Animate;

            if (name != null)
            {
                if (Animations.TryGetValue(name, out var oldAnimation))
                {
                    oldAnimation.Stop();
                    await oldAnimation.Wait();
                }
                Animations.Add(name, animation);
            }

            stopwatch?.Start();
            await tcs.Task;
        }
        public static Task AnimateMatrixAlongPath(string name, Action<Matrix> setter, double from, double to, Geometry geometry, bool doesRotateWithTangent, TimeSpan duration, RepeatBehavior repeatBehavior = default, bool autoReverse = false, bool isOffsetCumulative = false, bool isAngleCumulative = false, IEasingFunction easingFunction = null, int? fps = null) => AnimateMatrixAlongPath(name, setter, from, to, PathGeometry.CreateFromGeometry(geometry), doesRotateWithTangent, duration, repeatBehavior, autoReverse, isOffsetCumulative, isAngleCumulative, easingFunction, fps);

        public static async Task AnimateMatrixAlongPathUsingKeyFrames(string name, Action<Matrix> setter, double from, IEnumerable<DoubleKeyFrame> keyFrames, PathGeometry pathGeometry, bool doesRotateWithTangent, Duration duration, RepeatBehavior repeatBehavior = default, bool autoReverse = false, bool isOffsetCumulative = false, bool isAngleCumulative = false, int? fps = null)
        {
            bool finished = false;
            var tcs = new TaskCompletionSource<object>();
            var animation = new StaticAnimation();
            long iterationsCount = 0;

            Stopwatch stopwatch = null;
            FrameStopwatch frameStopwatch = null;
            if (fps.HasValue) frameStopwatch = new FrameStopwatch(fps.Value, 0);
            else stopwatch = new Stopwatch();

            TimeSpan CurrentTime() => fps.HasValue ? frameStopwatch.Elapsed : stopwatch.Elapsed;

            var resolvedKeyFrames = ResolveKeyTimes(keyFrames, duration, out var durationTime);

            //=============================================================================================
            Vector pointDiff = default;
            double angleDiff = 0.0;

            pathGeometry.GetPointAtFractionLength(resolvedKeyFrames[0].Value, out var firstPoint, out var firstTangent);
            pathGeometry.GetPointAtFractionLength(resolvedKeyFrames[resolvedKeyFrames.Length - 1].Value, out var lastPoint, out var lastTangent);

            if (isOffsetCumulative)
            {
                pointDiff.X = lastPoint.X - firstPoint.X;
                pointDiff.Y = lastPoint.Y - firstPoint.Y;
            }
            if (isAngleCumulative && doesRotateWithTangent) angleDiff = CalculateAngleFromTangentVector(firstTangent.X, firstTangent.Y) - CalculateAngleFromTangentVector(lastTangent.X, lastTangent.Y);
            //=============================================================================================

            TimeSpan totalDuration;
            if (repeatBehavior.HasCount) totalDuration = repeatBehavior.Count == 0 ? durationTime : durationTime.Multiply(repeatBehavior.Count);
            else if (repeatBehavior.HasDuration) totalDuration = repeatBehavior.Duration;
            else totalDuration = TimeSpan.MaxValue;

            if (name != null) animation.StateChanged += StateChanged;

            void StateChanged(object sender, EventArgs<StaticAnimationState> e)
            {
                switch (e.Param1)
                {
                    case StaticAnimationState.Running:
                        stopwatch?.Start();
                        CompositionTarget.Rendering += Animate;
                        break;
                    case StaticAnimationState.Paused:
                        stopwatch?.Stop();
                        CompositionTarget.Rendering -= Animate;
                        break;
                    case StaticAnimationState.Stopped:
                        Finish();
                        break;
                }
            }

            void Animate(object sender, EventArgs e)
            {
                var currentTime = CurrentTime();
                if (currentTime > durationTime.Multiply(iterationsCount + 1) && currentTime < totalDuration) iterationsCount++;

                currentTime -= durationTime.Multiply(iterationsCount);
                if (autoReverse && iterationsCount % 2 != 0) currentTime = -currentTime + durationTime;

                int maxKeyFrameIndex = resolvedKeyFrames.Length - 1;
                int currentKeyFrameIndex = 0;
                double currentKeyFrameValue;

                while (currentKeyFrameIndex < resolvedKeyFrames.Length && currentTime > resolvedKeyFrames[currentKeyFrameIndex].KeyTime.TimeSpan) currentKeyFrameIndex++;
                while (currentKeyFrameIndex < maxKeyFrameIndex && currentTime == resolvedKeyFrames[currentKeyFrameIndex + 1].KeyTime.TimeSpan) currentKeyFrameIndex++;

                if (currentKeyFrameIndex == resolvedKeyFrames.Length) currentKeyFrameValue = resolvedKeyFrames[maxKeyFrameIndex].Value;
                else if (currentTime == resolvedKeyFrames[currentKeyFrameIndex].KeyTime.TimeSpan) currentKeyFrameValue = resolvedKeyFrames[currentKeyFrameIndex].Value;
                else
                {
                    double fromValue;
                    double currentSegmentProgress;

                    if (currentKeyFrameIndex == 0)
                    {
                        fromValue = from;
                        currentSegmentProgress = currentTime.TotalMilliseconds / resolvedKeyFrames[0].KeyTime.TimeSpan.TotalMilliseconds;
                    }
                    else
                    {
                        int previousKeyFrameIndex = currentKeyFrameIndex - 1;
                        var previousKeyTime = resolvedKeyFrames[previousKeyFrameIndex].KeyTime.TimeSpan;

                        fromValue = resolvedKeyFrames[previousKeyFrameIndex].Value;

                        var segmentCurrentTime = currentTime - previousKeyTime;
                        var segmentDuration = resolvedKeyFrames[currentKeyFrameIndex].KeyTime.TimeSpan - previousKeyTime;

                        currentSegmentProgress = segmentCurrentTime.TotalMilliseconds / segmentDuration.TotalMilliseconds;
                    }

                    currentKeyFrameValue = resolvedKeyFrames[currentKeyFrameIndex].InterpolateValue(fromValue, Math.Min(currentSegmentProgress, 1));
                }

                //=============================================================================================
                pathGeometry.GetPointAtFractionLength(currentKeyFrameValue, out var point, out var tangent);
                double angle = doesRotateWithTangent ? CalculateAngleFromTangentVector(tangent.X, tangent.Y) : 0.0;

                var matrix = new Matrix();

                if (isOffsetCumulative) point += pointDiff * iterationsCount;
                if (isAngleCumulative && doesRotateWithTangent) angle += angleDiff * iterationsCount;

                matrix.Rotate(angle);
                matrix.Translate(point.X, point.Y);

                setter(matrix);
                //=============================================================================================

                frameStopwatch?.Spend();

                if (currentTime >= totalDuration) Finish();
            }

            void Finish()
            {
                if (!finished)
                {
                    finished = true;
                    stopwatch?.Stop();
                    CompositionTarget.Rendering -= Animate;
                    if (name != null)
                    {
                        animation.StateChanged -= StateChanged;
                        animation.End();
                        Animations.Remove(name);
                    }
                    tcs.TrySetResult(null);
                }
            }

            CompositionTarget.Rendering += Animate;

            if (name != null)
            {
                if (Animations.TryGetValue(name, out var oldAnimation))
                {
                    oldAnimation.Stop();
                    await oldAnimation.Wait();
                }
                Animations.Add(name, animation);
            }

            stopwatch?.Start();
            await tcs.Task;
        }
        public static Task AnimateMatrixAlongPathUsingKeyFrames(string name, Action<Matrix> setter, double from, IEnumerable<DoubleKeyFrame> keyFrames, Geometry geometry, bool doesRotateWithTangent, Duration duration, RepeatBehavior repeatBehavior = default, bool autoReverse = false, bool isOffsetCumulative = false, bool isAngleCumulative = false, int? fps = null) => AnimateMatrixAlongPathUsingKeyFrames(name, setter, from, keyFrames, PathGeometry.CreateFromGeometry(geometry), doesRotateWithTangent, duration, repeatBehavior, autoReverse, isOffsetCumulative, isAngleCumulative, fps);

        private static double CalculateAngleFromTangentVector(double x, double y)
        {
            double angle = Math.Acos(x) * (180.0 / Math.PI);
            if (y < 0.0) angle = 360 - angle;
            return angle;
        }

        #endregion

        #region RectAnimation

        public static async Task AnimateRect(string name, Action<Rect> setter, Rect from, Rect to, TimeSpan duration, RepeatBehavior repeatBehavior = default, bool autoReverse = false, bool isCumulative = false, IEasingFunction easingFunction = null, int? fps = null)
        {
            bool finished = false;
            var tcs = new TaskCompletionSource<object>();
            var animation = new StaticAnimation();
            long iterationsCount = 0;

            double durationMilliseconds = duration.TotalMilliseconds;

            Stopwatch stopwatch = null;
            FrameStopwatch frameStopwatch = null;
            if (fps.HasValue) frameStopwatch = new FrameStopwatch(fps.Value, 0);
            else stopwatch = new Stopwatch();

            double CurrentMilliseconds() => fps.HasValue ? frameStopwatch.ElapsedMilliseconds : stopwatch.ElapsedMilliseconds;

            TimeSpan totalDuration;
            if (repeatBehavior.HasCount) totalDuration = repeatBehavior.Count == 0 ? duration : duration.Multiply(repeatBehavior.Count);
            else if (repeatBehavior.HasDuration) totalDuration = repeatBehavior.Duration;
            else totalDuration = TimeSpan.MaxValue;

            double totalMilliseconds = totalDuration.TotalMilliseconds;

            //=============================================================================================
            double fromX = from.X;
            double fromY = from.Y;
            double fromWidth = from.Width;
            double fromHeight = from.Height;

            double diffX = to.X - from.X;
            double diffY = to.Y - from.Y;
            double diffWidth = to.Width - from.Width;
            double diffHeight = to.Height - from.Height;
            //=============================================================================================

            if (name != null) animation.StateChanged += StateChanged;

            void StateChanged(object sender, EventArgs<StaticAnimationState> e)
            {
                switch (e.Param1)
                {
                    case StaticAnimationState.Running:
                        stopwatch?.Start();
                        CompositionTarget.Rendering += Animate;
                        break;
                    case StaticAnimationState.Paused:
                        stopwatch?.Stop();
                        CompositionTarget.Rendering -= Animate;
                        break;
                    case StaticAnimationState.Stopped:
                        Finish();
                        break;
                }
            }

            void Animate(object sender, EventArgs e)
            {
                double baseCurrentMilliseconds = CurrentMilliseconds();
                double currentMilliseconds = baseCurrentMilliseconds;
                if (currentMilliseconds > (iterationsCount + 1) * durationMilliseconds && currentMilliseconds < totalMilliseconds) iterationsCount++;

                currentMilliseconds -= durationMilliseconds * iterationsCount;
                if (autoReverse && iterationsCount % 2 != 0) currentMilliseconds = -currentMilliseconds + durationMilliseconds;

                double baseProgress = Math.Min(currentMilliseconds / durationMilliseconds, 1);
                double progress = easingFunction?.Ease(baseProgress) ?? baseProgress;

                //=============================================================================================
                var value = new Rect(fromX + diffX * progress, fromY + diffY * progress, fromWidth + diffWidth * progress, fromHeight + diffHeight * progress);

                if (isCumulative)
                {
                    value.X += diffX * iterationsCount;
                    value.Y += diffY * iterationsCount;
                    value.Width += diffWidth * iterationsCount;
                    value.Height += diffHeight * iterationsCount;
                }

                setter(value);
                //=============================================================================================

                frameStopwatch?.Spend();

                if (baseCurrentMilliseconds >= totalMilliseconds) Finish();
            }

            void Finish()
            {
                if (!finished)
                {
                    finished = true;
                    stopwatch?.Stop();
                    CompositionTarget.Rendering -= Animate;
                    if (name != null)
                    {
                        animation.StateChanged -= StateChanged;
                        Animations.Remove(name);
                        animation.End();
                    }
                    tcs.TrySetResult(null);
                }
            }

            CompositionTarget.Rendering += Animate;

            if (name != null)
            {
                if (Animations.TryGetValue(name, out var oldAnimation))
                {
                    oldAnimation.Stop();
                    await oldAnimation.Wait();
                }
                Animations.Add(name, animation);
            }

            stopwatch?.Start();
            await tcs.Task;
        }

        public static async Task AnimateRectArray(string name, Action<Rect[]> setter, IList<Rect> from, IList<Rect> to, TimeSpan duration, RepeatBehavior repeatBehavior = default, bool autoReverse = false, bool isCumulative = false, IEasingFunction easingFunction = null, int? fps = null)
        {
            bool finished = false;
            var tcs = new TaskCompletionSource<object>();
            var animation = new StaticAnimation();
            long iterationsCount = 0;

            double durationMilliseconds = duration.TotalMilliseconds;

            Stopwatch stopwatch = null;
            FrameStopwatch frameStopwatch = null;
            if (fps.HasValue) frameStopwatch = new FrameStopwatch(fps.Value, 0);
            else stopwatch = new Stopwatch();

            double CurrentMilliseconds() => fps.HasValue ? frameStopwatch.ElapsedMilliseconds : stopwatch.ElapsedMilliseconds;

            TimeSpan totalDuration;
            if (repeatBehavior.HasCount) totalDuration = repeatBehavior.Count == 0 ? duration : duration.Multiply(repeatBehavior.Count);
            else if (repeatBehavior.HasDuration) totalDuration = repeatBehavior.Duration;
            else totalDuration = TimeSpan.MaxValue;

            double totalMilliseconds = totalDuration.TotalMilliseconds;

            //=============================================================================================
            int count = from.Count;
            var diff = new Rect[count];

            for (int i = 0; i < count; i++) diff[i] = new Rect(to[i].X - from[i].X, to[i].Y - from[i].Y, to[i].Width - from[i].Width, to[i].Height - from[i].Height);
            //=============================================================================================

            if (name != null) animation.StateChanged += StateChanged;

            void StateChanged(object sender, EventArgs<StaticAnimationState> e)
            {
                switch (e.Param1)
                {
                    case StaticAnimationState.Running:
                        stopwatch?.Start();
                        CompositionTarget.Rendering += Animate;
                        break;
                    case StaticAnimationState.Paused:
                        stopwatch?.Stop();
                        CompositionTarget.Rendering -= Animate;
                        break;
                    case StaticAnimationState.Stopped:
                        Finish();
                        break;
                }
            }

            void Animate(object sender, EventArgs e)
            {
                double baseCurrentMilliseconds = CurrentMilliseconds();
                double currentMilliseconds = baseCurrentMilliseconds;
                if (currentMilliseconds > (iterationsCount + 1) * durationMilliseconds && currentMilliseconds < totalMilliseconds) iterationsCount++;

                currentMilliseconds -= durationMilliseconds * iterationsCount;
                if (autoReverse && iterationsCount % 2 != 0) currentMilliseconds = -currentMilliseconds + durationMilliseconds;

                double baseProgress = Math.Min(currentMilliseconds / durationMilliseconds, 1);
                double progress = easingFunction?.Ease(baseProgress) ?? baseProgress;

                //=============================================================================================
                var value = new Rect[count];

                for (int i = 0; i < count; i++)
                {
                    var dif = diff[i];
                    var val = new Rect(from[i].X + diff[i].X * progress, from[i].Y + diff[i].Y * progress, from[i].Width + diff[i].Width * progress, from[i].Height + diff[i].Height * progress);
                    if (isCumulative)
                    {
                        val.X += dif.X * iterationsCount;
                        val.Y += dif.Y * iterationsCount;
                        val.Width += dif.Width * iterationsCount;
                        val.Height += dif.Height * iterationsCount;
                    }

                    value[i] = val;
                }

                setter(value);
                //=============================================================================================

                frameStopwatch?.Spend();

                if (baseCurrentMilliseconds >= totalMilliseconds) Finish();
            }

            void Finish()
            {
                if (!finished)
                {
                    finished = true;
                    stopwatch?.Stop();
                    CompositionTarget.Rendering -= Animate;
                    if (name != null)
                    {
                        animation.StateChanged -= StateChanged;
                        Animations.Remove(name);
                        animation.End();
                    }
                    tcs.TrySetResult(null);
                }
            }

            CompositionTarget.Rendering += Animate;

            if (name != null)
            {
                if (Animations.TryGetValue(name, out var oldAnimation))
                {
                    oldAnimation.Stop();
                    await oldAnimation.Wait();
                }
                Animations.Add(name, animation);
            }

            stopwatch?.Start();
            await tcs.Task;
        }

        public static async Task AnimateRectUsingKeyFrames(string name, Action<Rect> setter, Rect from, IEnumerable<RectKeyFrame> keyFrames, Duration duration, RepeatBehavior repeatBehavior = default, bool autoReverse = false, bool isCumulative = false, int? fps = null)
        {
            bool finished = false;
            var tcs = new TaskCompletionSource<object>();
            var animation = new StaticAnimation();
            long iterationsCount = 0;

            Stopwatch stopwatch = null;
            FrameStopwatch frameStopwatch = null;
            if (fps.HasValue) frameStopwatch = new FrameStopwatch(fps.Value, 0);
            else stopwatch = new Stopwatch();

            TimeSpan CurrentTime() => fps.HasValue ? frameStopwatch.Elapsed : stopwatch.Elapsed;

            var resolvedKeyFrames = ResolveKeyTimes(keyFrames, duration, out var durationTime);

            //=============================================================================================
            var firstValue = resolvedKeyFrames[0].Value;
            var lastValue = resolvedKeyFrames[resolvedKeyFrames.Length - 1].Value;
            var diff = new Rect(lastValue.X - firstValue.X, lastValue.Y - firstValue.Y, lastValue.Width - firstValue.Width, lastValue.Height - firstValue.Height);
            //=============================================================================================

            TimeSpan totalDuration;
            if (repeatBehavior.HasCount) totalDuration = repeatBehavior.Count == 0 ? durationTime : durationTime.Multiply(repeatBehavior.Count);
            else if (repeatBehavior.HasDuration) totalDuration = repeatBehavior.Duration;
            else totalDuration = TimeSpan.MaxValue;

            if (name != null) animation.StateChanged += StateChanged;

            void StateChanged(object sender, EventArgs<StaticAnimationState> e)
            {
                switch (e.Param1)
                {
                    case StaticAnimationState.Running:
                        stopwatch?.Start();
                        CompositionTarget.Rendering += Animate;
                        break;
                    case StaticAnimationState.Paused:
                        stopwatch?.Stop();
                        CompositionTarget.Rendering -= Animate;
                        break;
                    case StaticAnimationState.Stopped:
                        Finish();
                        break;
                }
            }

            void Animate(object sender, EventArgs e)
            {
                var currentTime = CurrentTime();
                if (currentTime > durationTime.Multiply(iterationsCount + 1) && currentTime < totalDuration) iterationsCount++;

                currentTime -= durationTime.Multiply(iterationsCount);
                if (autoReverse && iterationsCount % 2 != 0) currentTime = -currentTime + durationTime;

                int maxKeyFrameIndex = resolvedKeyFrames.Length - 1;
                int currentKeyFrameIndex = 0;
                Rect currentKeyFrameValue;

                while (currentKeyFrameIndex < resolvedKeyFrames.Length && currentTime > resolvedKeyFrames[currentKeyFrameIndex].KeyTime.TimeSpan) currentKeyFrameIndex++;
                while (currentKeyFrameIndex < maxKeyFrameIndex && currentTime == resolvedKeyFrames[currentKeyFrameIndex + 1].KeyTime.TimeSpan) currentKeyFrameIndex++;

                if (currentKeyFrameIndex == resolvedKeyFrames.Length) currentKeyFrameValue = resolvedKeyFrames[maxKeyFrameIndex].Value;
                else if (currentTime == resolvedKeyFrames[currentKeyFrameIndex].KeyTime.TimeSpan) currentKeyFrameValue = resolvedKeyFrames[currentKeyFrameIndex].Value;
                else
                {
                    Rect fromValue;
                    double currentSegmentProgress;

                    if (currentKeyFrameIndex == 0)
                    {
                        fromValue = from;
                        currentSegmentProgress = currentTime.TotalMilliseconds / resolvedKeyFrames[0].KeyTime.TimeSpan.TotalMilliseconds;
                    }
                    else
                    {
                        int previousKeyFrameIndex = currentKeyFrameIndex - 1;
                        var previousKeyTime = resolvedKeyFrames[previousKeyFrameIndex].KeyTime.TimeSpan;

                        fromValue = resolvedKeyFrames[previousKeyFrameIndex].Value;

                        var segmentCurrentTime = currentTime - previousKeyTime;
                        var segmentDuration = resolvedKeyFrames[currentKeyFrameIndex].KeyTime.TimeSpan - previousKeyTime;

                        currentSegmentProgress = segmentCurrentTime.TotalMilliseconds / segmentDuration.TotalMilliseconds;
                    }

                    currentKeyFrameValue = resolvedKeyFrames[currentKeyFrameIndex].InterpolateValue(fromValue, Math.Min(currentSegmentProgress, 1));
                }

                //=============================================================================================
                if (isCumulative)
                {
                    currentKeyFrameValue.X += diff.X * iterationsCount;
                    currentKeyFrameValue.Y += diff.Y * iterationsCount;
                    currentKeyFrameValue.Width += diff.Width * iterationsCount;
                    currentKeyFrameValue.Height += diff.Height * iterationsCount;
                }

                setter(currentKeyFrameValue);
                //=============================================================================================

                frameStopwatch?.Spend();

                if (currentTime >= totalDuration) Finish();
            }

            void Finish()
            {
                if (!finished)
                {
                    finished = true;
                    stopwatch?.Stop();
                    CompositionTarget.Rendering -= Animate;
                    if (name != null)
                    {
                        animation.StateChanged -= StateChanged;
                        animation.End();
                        Animations.Remove(name);
                    }
                    tcs.TrySetResult(null);
                }
            }

            CompositionTarget.Rendering += Animate;

            if (name != null)
            {
                if (Animations.TryGetValue(name, out var oldAnimation))
                {
                    oldAnimation.Stop();
                    await oldAnimation.Wait();
                }
                Animations.Add(name, animation);
            }

            stopwatch?.Start();
            await tcs.Task;
        }

        #endregion

        #region VectorAnimation

        public static async Task AnimateVector(string name, Action<Vector> setter, Vector from, Vector to, TimeSpan duration, RepeatBehavior repeatBehavior = default, bool autoReverse = false, bool isCumulative = false, IEasingFunction easingFunction = null, int? fps = null)
        {
            bool finished = false;
            var tcs = new TaskCompletionSource<object>();
            var animation = new StaticAnimation();
            long iterationsCount = 0;

            double durationMilliseconds = duration.TotalMilliseconds;

            Stopwatch stopwatch = null;
            FrameStopwatch frameStopwatch = null;
            if (fps.HasValue) frameStopwatch = new FrameStopwatch(fps.Value, 0);
            else stopwatch = new Stopwatch();

            double CurrentMilliseconds() => fps.HasValue ? frameStopwatch.ElapsedMilliseconds : stopwatch.ElapsedMilliseconds;

            TimeSpan totalDuration;
            if (repeatBehavior.HasCount) totalDuration = repeatBehavior.Count == 0 ? duration : duration.Multiply(repeatBehavior.Count);
            else if (repeatBehavior.HasDuration) totalDuration = repeatBehavior.Duration;
            else totalDuration = TimeSpan.MaxValue;

            double totalMilliseconds = totalDuration.TotalMilliseconds;

            //=============================================================================================
            var diff = to - from;
            //=============================================================================================

            if (name != null) animation.StateChanged += StateChanged;

            void StateChanged(object sender, EventArgs<StaticAnimationState> e)
            {
                switch (e.Param1)
                {
                    case StaticAnimationState.Running:
                        stopwatch?.Start();
                        CompositionTarget.Rendering += Animate;
                        break;
                    case StaticAnimationState.Paused:
                        stopwatch?.Stop();
                        CompositionTarget.Rendering -= Animate;
                        break;
                    case StaticAnimationState.Stopped:
                        Finish();
                        break;
                }
            }

            void Animate(object sender, EventArgs e)
            {
                double baseCurrentMilliseconds = CurrentMilliseconds();
                double currentMilliseconds = baseCurrentMilliseconds;
                if (currentMilliseconds > (iterationsCount + 1) * durationMilliseconds && currentMilliseconds < totalMilliseconds) iterationsCount++;

                currentMilliseconds -= durationMilliseconds * iterationsCount;
                if (autoReverse && iterationsCount % 2 != 0) currentMilliseconds = -currentMilliseconds + durationMilliseconds;

                double baseProgress = Math.Min(currentMilliseconds / durationMilliseconds, 1);
                double progress = easingFunction?.Ease(baseProgress) ?? baseProgress;

                //=============================================================================================
                var value = from + diff * progress;
                if (isCumulative) value += diff * iterationsCount;

                setter(value);
                //=============================================================================================

                frameStopwatch?.Spend();

                if (baseCurrentMilliseconds >= totalMilliseconds) Finish();
            }

            void Finish()
            {
                if (!finished)
                {
                    finished = true;
                    stopwatch?.Stop();
                    CompositionTarget.Rendering -= Animate;
                    if (name != null)
                    {
                        animation.StateChanged -= StateChanged;
                        Animations.Remove(name);
                        animation.End();
                    }
                    tcs.TrySetResult(null);
                }
            }

            CompositionTarget.Rendering += Animate;

            if (name != null)
            {
                if (Animations.TryGetValue(name, out var oldAnimation))
                {
                    oldAnimation.Stop();
                    await oldAnimation.Wait();
                }
                Animations.Add(name, animation);
            }

            stopwatch?.Start();
            await tcs.Task;
        }

        public static async Task AnimateVectorArray(string name, Action<Vector[]> setter, IList<Vector> from, IList<Vector> to, TimeSpan duration, RepeatBehavior repeatBehavior = default, bool autoReverse = false, bool isCumulative = false, IEasingFunction easingFunction = null, int? fps = null)
        {
            bool finished = false;
            var tcs = new TaskCompletionSource<object>();
            var animation = new StaticAnimation();
            long iterationsCount = 0;

            double durationMilliseconds = duration.TotalMilliseconds;

            Stopwatch stopwatch = null;
            FrameStopwatch frameStopwatch = null;
            if (fps.HasValue) frameStopwatch = new FrameStopwatch(fps.Value, 0);
            else stopwatch = new Stopwatch();

            double CurrentMilliseconds() => fps.HasValue ? frameStopwatch.ElapsedMilliseconds : stopwatch.ElapsedMilliseconds;

            TimeSpan totalDuration;
            if (repeatBehavior.HasCount) totalDuration = repeatBehavior.Count == 0 ? duration : duration.Multiply(repeatBehavior.Count);
            else if (repeatBehavior.HasDuration) totalDuration = repeatBehavior.Duration;
            else totalDuration = TimeSpan.MaxValue;

            double totalMilliseconds = totalDuration.TotalMilliseconds;

            //=============================================================================================
            int count = from.Count;
            var diff = new Vector[count];

            for (int i = 0; i < count; i++) diff[i] = to[i] - from[i];
            //=============================================================================================

            if (name != null) animation.StateChanged += StateChanged;

            void StateChanged(object sender, EventArgs<StaticAnimationState> e)
            {
                switch (e.Param1)
                {
                    case StaticAnimationState.Running:
                        stopwatch?.Start();
                        CompositionTarget.Rendering += Animate;
                        break;
                    case StaticAnimationState.Paused:
                        stopwatch?.Stop();
                        CompositionTarget.Rendering -= Animate;
                        break;
                    case StaticAnimationState.Stopped:
                        Finish();
                        break;
                }
            }

            void Animate(object sender, EventArgs e)
            {
                double baseCurrentMilliseconds = CurrentMilliseconds();
                double currentMilliseconds = baseCurrentMilliseconds;
                if (currentMilliseconds > (iterationsCount + 1) * durationMilliseconds && currentMilliseconds < totalMilliseconds) iterationsCount++;

                currentMilliseconds -= durationMilliseconds * iterationsCount;
                if (autoReverse && iterationsCount % 2 != 0) currentMilliseconds = -currentMilliseconds + durationMilliseconds;

                double baseProgress = Math.Min(currentMilliseconds / durationMilliseconds, 1);
                double progress = easingFunction?.Ease(baseProgress) ?? baseProgress;

                //=============================================================================================
                var value = new Vector[count];
                for (int i = 0; i < count; i++)
                {
                    var dif = diff[i];
                    var val = from[i] + dif * progress;
                    if (isCumulative) val += dif * iterationsCount;

                    value[i] = val;
                }

                setter(value);
                //=============================================================================================

                frameStopwatch?.Spend();

                if (baseCurrentMilliseconds >= totalMilliseconds) Finish();
            }

            void Finish()
            {
                if (!finished)
                {
                    finished = true;
                    stopwatch?.Stop();
                    CompositionTarget.Rendering -= Animate;
                    if (name != null)
                    {
                        animation.StateChanged -= StateChanged;
                        Animations.Remove(name);
                        animation.End();
                    }
                    tcs.TrySetResult(null);
                }
            }

            CompositionTarget.Rendering += Animate;

            if (name != null)
            {
                if (Animations.TryGetValue(name, out var oldAnimation))
                {
                    oldAnimation.Stop();
                    await oldAnimation.Wait();
                }
                Animations.Add(name, animation);
            }

            stopwatch?.Start();
            await tcs.Task;
        }

        public static async Task AnimateVectorUsingKeyFrames(string name, Action<Vector> setter, Vector from, IEnumerable<VectorKeyFrame> keyFrames, Duration duration, RepeatBehavior repeatBehavior = default, bool autoReverse = false, bool isCumulative = false, int? fps = null)
        {
            bool finished = false;
            var tcs = new TaskCompletionSource<object>();
            var animation = new StaticAnimation();
            long iterationsCount = 0;

            Stopwatch stopwatch = null;
            FrameStopwatch frameStopwatch = null;
            if (fps.HasValue) frameStopwatch = new FrameStopwatch(fps.Value, 0);
            else stopwatch = new Stopwatch();

            TimeSpan CurrentTime() => fps.HasValue ? frameStopwatch.Elapsed : stopwatch.Elapsed;

            var resolvedKeyFrames = ResolveKeyTimes(keyFrames, duration, out var durationTime);

            //=============================================================================================
            var diff = resolvedKeyFrames[resolvedKeyFrames.Length - 1].Value - resolvedKeyFrames[0].Value;
            //=============================================================================================

            TimeSpan totalDuration;
            if (repeatBehavior.HasCount) totalDuration = repeatBehavior.Count == 0 ? durationTime : durationTime.Multiply(repeatBehavior.Count);
            else if (repeatBehavior.HasDuration) totalDuration = repeatBehavior.Duration;
            else totalDuration = TimeSpan.MaxValue;

            if (name != null) animation.StateChanged += StateChanged;

            void StateChanged(object sender, EventArgs<StaticAnimationState> e)
            {
                switch (e.Param1)
                {
                    case StaticAnimationState.Running:
                        stopwatch?.Start();
                        CompositionTarget.Rendering += Animate;
                        break;
                    case StaticAnimationState.Paused:
                        stopwatch?.Stop();
                        CompositionTarget.Rendering -= Animate;
                        break;
                    case StaticAnimationState.Stopped:
                        Finish();
                        break;
                }
            }

            void Animate(object sender, EventArgs e)
            {
                var currentTime = CurrentTime();
                if (currentTime > durationTime.Multiply(iterationsCount + 1) && currentTime < totalDuration) iterationsCount++;

                currentTime -= durationTime.Multiply(iterationsCount);
                if (autoReverse && iterationsCount % 2 != 0) currentTime = -currentTime + durationTime;

                int maxKeyFrameIndex = resolvedKeyFrames.Length - 1;
                int currentKeyFrameIndex = 0;
                Vector currentKeyFrameValue;

                while (currentKeyFrameIndex < resolvedKeyFrames.Length && currentTime > resolvedKeyFrames[currentKeyFrameIndex].KeyTime.TimeSpan) currentKeyFrameIndex++;
                while (currentKeyFrameIndex < maxKeyFrameIndex && currentTime == resolvedKeyFrames[currentKeyFrameIndex + 1].KeyTime.TimeSpan) currentKeyFrameIndex++;

                if (currentKeyFrameIndex == resolvedKeyFrames.Length) currentKeyFrameValue = resolvedKeyFrames[maxKeyFrameIndex].Value;
                else if (currentTime == resolvedKeyFrames[currentKeyFrameIndex].KeyTime.TimeSpan) currentKeyFrameValue = resolvedKeyFrames[currentKeyFrameIndex].Value;
                else
                {
                    Vector fromValue;
                    double currentSegmentProgress;

                    if (currentKeyFrameIndex == 0)
                    {
                        fromValue = from;
                        currentSegmentProgress = currentTime.TotalMilliseconds / resolvedKeyFrames[0].KeyTime.TimeSpan.TotalMilliseconds;
                    }
                    else
                    {
                        int previousKeyFrameIndex = currentKeyFrameIndex - 1;
                        var previousKeyTime = resolvedKeyFrames[previousKeyFrameIndex].KeyTime.TimeSpan;

                        fromValue = resolvedKeyFrames[previousKeyFrameIndex].Value;

                        var segmentCurrentTime = currentTime - previousKeyTime;
                        var segmentDuration = resolvedKeyFrames[currentKeyFrameIndex].KeyTime.TimeSpan - previousKeyTime;

                        currentSegmentProgress = segmentCurrentTime.TotalMilliseconds / segmentDuration.TotalMilliseconds;
                    }

                    currentKeyFrameValue = resolvedKeyFrames[currentKeyFrameIndex].InterpolateValue(fromValue, Math.Min(currentSegmentProgress, 1));
                }

                //=============================================================================================
                if (isCumulative) currentKeyFrameValue += diff * iterationsCount;

                setter(currentKeyFrameValue);
                //=============================================================================================

                frameStopwatch?.Spend();

                if (currentTime >= totalDuration) Finish();
            }

            void Finish()
            {
                if (!finished)
                {
                    finished = true;
                    stopwatch?.Stop();
                    CompositionTarget.Rendering -= Animate;
                    if (name != null)
                    {
                        animation.StateChanged -= StateChanged;
                        animation.End();
                        Animations.Remove(name);
                    }
                    tcs.TrySetResult(null);
                }
            }

            CompositionTarget.Rendering += Animate;

            if (name != null)
            {
                if (Animations.TryGetValue(name, out var oldAnimation))
                {
                    oldAnimation.Stop();
                    await oldAnimation.Wait();
                }
                Animations.Add(name, animation);
            }

            stopwatch?.Start();
            await tcs.Task;
        }

        #endregion

        #region ColorAnimation

        public static async Task AnimateColor(string name, Action<Color> setter, Color from, Color to, TimeSpan duration, RepeatBehavior repeatBehavior = default, bool autoReverse = false, bool isCumulative = false, IEasingFunction easingFunction = null, int? fps = null)
        {
            bool finished = false;
            var tcs = new TaskCompletionSource<object>();
            var animation = new StaticAnimation();
            long iterationsCount = 0;

            double durationMilliseconds = duration.TotalMilliseconds;

            Stopwatch stopwatch = null;
            FrameStopwatch frameStopwatch = null;
            if (fps.HasValue) frameStopwatch = new FrameStopwatch(fps.Value, 0);
            else stopwatch = new Stopwatch();

            double CurrentMilliseconds() => fps.HasValue ? frameStopwatch.ElapsedMilliseconds : stopwatch.ElapsedMilliseconds;

            TimeSpan totalDuration;
            if (repeatBehavior.HasCount) totalDuration = repeatBehavior.Count == 0 ? duration : duration.Multiply(repeatBehavior.Count);
            else if (repeatBehavior.HasDuration) totalDuration = repeatBehavior.Duration;
            else totalDuration = TimeSpan.MaxValue;

            double totalMilliseconds = totalDuration.TotalMilliseconds;

            //=============================================================================================
            var diff = to - from;
            //=============================================================================================

            if (name != null) animation.StateChanged += StateChanged;

            void StateChanged(object sender, EventArgs<StaticAnimationState> e)
            {
                switch (e.Param1)
                {
                    case StaticAnimationState.Running:
                        stopwatch?.Start();
                        CompositionTarget.Rendering += Animate;
                        break;
                    case StaticAnimationState.Paused:
                        stopwatch?.Stop();
                        CompositionTarget.Rendering -= Animate;
                        break;
                    case StaticAnimationState.Stopped:
                        Finish();
                        break;
                }
            }

            void Animate(object sender, EventArgs e)
            {
                double baseCurrentMilliseconds = CurrentMilliseconds();
                double currentMilliseconds = baseCurrentMilliseconds;
                if (currentMilliseconds > (iterationsCount + 1) * durationMilliseconds && currentMilliseconds < totalMilliseconds) iterationsCount++;

                currentMilliseconds -= durationMilliseconds * iterationsCount;
                if (autoReverse && iterationsCount % 2 != 0) currentMilliseconds = -currentMilliseconds + durationMilliseconds;

                double baseProgress = Math.Min(currentMilliseconds / durationMilliseconds, 1);
                double progress = easingFunction?.Ease(baseProgress) ?? baseProgress;

                //=============================================================================================
                var value = from + diff * (float)progress;
                if (isCumulative) value += diff * iterationsCount;

                setter(value);
                //=============================================================================================

                frameStopwatch?.Spend();

                if (baseCurrentMilliseconds >= totalMilliseconds) Finish();
            }

            void Finish()
            {
                if (!finished)
                {
                    finished = true;
                    stopwatch?.Stop();
                    CompositionTarget.Rendering -= Animate;
                    if (name != null)
                    {
                        animation.StateChanged -= StateChanged;
                        Animations.Remove(name);
                        animation.End();
                    }
                    tcs.TrySetResult(null);
                }
            }

            CompositionTarget.Rendering += Animate;

            if (name != null)
            {
                if (Animations.TryGetValue(name, out var oldAnimation))
                {
                    oldAnimation.Stop();
                    await oldAnimation.Wait();
                }
                Animations.Add(name, animation);
            }

            stopwatch?.Start();
            await tcs.Task;
        }

        public static async Task AnimateColorArray(string name, Action<Color[]> setter, IList<Color> from, IList<Color> to, TimeSpan duration, RepeatBehavior repeatBehavior = default, bool autoReverse = false, bool isCumulative = false, IEasingFunction easingFunction = null, int? fps = null)
        {
            bool finished = false;
            var tcs = new TaskCompletionSource<object>();
            var animation = new StaticAnimation();
            long iterationsCount = 0;

            double durationMilliseconds = duration.TotalMilliseconds;

            Stopwatch stopwatch = null;
            FrameStopwatch frameStopwatch = null;
            if (fps.HasValue) frameStopwatch = new FrameStopwatch(fps.Value, 0);
            else stopwatch = new Stopwatch();

            double CurrentMilliseconds() => fps.HasValue ? frameStopwatch.ElapsedMilliseconds : stopwatch.ElapsedMilliseconds;

            TimeSpan totalDuration;
            if (repeatBehavior.HasCount) totalDuration = repeatBehavior.Count == 0 ? duration : duration.Multiply(repeatBehavior.Count);
            else if (repeatBehavior.HasDuration) totalDuration = repeatBehavior.Duration;
            else totalDuration = TimeSpan.MaxValue;

            double totalMilliseconds = totalDuration.TotalMilliseconds;

            //=============================================================================================
            int count = from.Count;
            var diff = new Color[count];

            for (int i = 0; i < count; i++) diff[i] = to[i] - from[i];
            //=============================================================================================

            if (name != null) animation.StateChanged += StateChanged;

            void StateChanged(object sender, EventArgs<StaticAnimationState> e)
            {
                switch (e.Param1)
                {
                    case StaticAnimationState.Running:
                        stopwatch?.Start();
                        CompositionTarget.Rendering += Animate;
                        break;
                    case StaticAnimationState.Paused:
                        stopwatch?.Stop();
                        CompositionTarget.Rendering -= Animate;
                        break;
                    case StaticAnimationState.Stopped:
                        Finish();
                        break;
                }
            }

            void Animate(object sender, EventArgs e)
            {
                double baseCurrentMilliseconds = CurrentMilliseconds();
                double currentMilliseconds = baseCurrentMilliseconds;
                if (currentMilliseconds > (iterationsCount + 1) * durationMilliseconds && currentMilliseconds < totalMilliseconds) iterationsCount++;

                currentMilliseconds -= durationMilliseconds * iterationsCount;
                if (autoReverse && iterationsCount % 2 != 0) currentMilliseconds = -currentMilliseconds + durationMilliseconds;

                double baseProgress = Math.Min(currentMilliseconds / durationMilliseconds, 1);
                double progress = easingFunction?.Ease(baseProgress) ?? baseProgress;

                //=============================================================================================
                var value = new Color[count];
                for (int i = 0; i < count; i++)
                {
                    var dif = diff[i];
                    var val = from[i] + dif * (float)progress;
                    if (isCumulative) val += dif * iterationsCount;

                    value[i] = val;
                }

                setter(value);
                //=============================================================================================

                frameStopwatch?.Spend();

                if (baseCurrentMilliseconds >= totalMilliseconds) Finish();
            }

            void Finish()
            {
                if (!finished)
                {
                    finished = true;
                    stopwatch?.Stop();
                    CompositionTarget.Rendering -= Animate;
                    if (name != null)
                    {
                        animation.StateChanged -= StateChanged;
                        Animations.Remove(name);
                        animation.End();
                    }
                    tcs.TrySetResult(null);
                }
            }

            CompositionTarget.Rendering += Animate;

            if (name != null)
            {
                if (Animations.TryGetValue(name, out var oldAnimation))
                {
                    oldAnimation.Stop();
                    await oldAnimation.Wait();
                }
                Animations.Add(name, animation);
            }

            stopwatch?.Start();
            await tcs.Task;
        }

        public static async Task AnimateColorUsingKeyFrames(string name, Action<Color> setter, Color from, IEnumerable<ColorKeyFrame> keyFrames, Duration duration, RepeatBehavior repeatBehavior = default, bool autoReverse = false, bool isCumulative = false, int? fps = null)
        {
            bool finished = false;
            var tcs = new TaskCompletionSource<object>();
            var animation = new StaticAnimation();
            long iterationsCount = 0;

            Stopwatch stopwatch = null;
            FrameStopwatch frameStopwatch = null;
            if (fps.HasValue) frameStopwatch = new FrameStopwatch(fps.Value, 0);
            else stopwatch = new Stopwatch();

            TimeSpan CurrentTime() => fps.HasValue ? frameStopwatch.Elapsed : stopwatch.Elapsed;

            var resolvedKeyFrames = ResolveKeyTimes(keyFrames, duration, out var durationTime);

            //=============================================================================================
            var diff = resolvedKeyFrames[resolvedKeyFrames.Length - 1].Value - resolvedKeyFrames[0].Value;
            //=============================================================================================

            TimeSpan totalDuration;
            if (repeatBehavior.HasCount) totalDuration = repeatBehavior.Count == 0 ? durationTime : durationTime.Multiply(repeatBehavior.Count);
            else if (repeatBehavior.HasDuration) totalDuration = repeatBehavior.Duration;
            else totalDuration = TimeSpan.MaxValue;

            if (name != null) animation.StateChanged += StateChanged;

            void StateChanged(object sender, EventArgs<StaticAnimationState> e)
            {
                switch (e.Param1)
                {
                    case StaticAnimationState.Running:
                        stopwatch?.Start();
                        CompositionTarget.Rendering += Animate;
                        break;
                    case StaticAnimationState.Paused:
                        stopwatch?.Stop();
                        CompositionTarget.Rendering -= Animate;
                        break;
                    case StaticAnimationState.Stopped:
                        Finish();
                        break;
                }
            }

            void Animate(object sender, EventArgs e)
            {
                var currentTime = CurrentTime();
                if (currentTime > durationTime.Multiply(iterationsCount + 1) && currentTime < totalDuration) iterationsCount++;

                currentTime -= durationTime.Multiply(iterationsCount);
                if (autoReverse && iterationsCount % 2 != 0) currentTime = -currentTime + durationTime;

                int maxKeyFrameIndex = resolvedKeyFrames.Length - 1;
                int currentKeyFrameIndex = 0;
                Color currentKeyFrameValue;

                while (currentKeyFrameIndex < resolvedKeyFrames.Length && currentTime > resolvedKeyFrames[currentKeyFrameIndex].KeyTime.TimeSpan) currentKeyFrameIndex++;
                while (currentKeyFrameIndex < maxKeyFrameIndex && currentTime == resolvedKeyFrames[currentKeyFrameIndex + 1].KeyTime.TimeSpan) currentKeyFrameIndex++;

                if (currentKeyFrameIndex == resolvedKeyFrames.Length) currentKeyFrameValue = resolvedKeyFrames[maxKeyFrameIndex].Value;
                else if (currentTime == resolvedKeyFrames[currentKeyFrameIndex].KeyTime.TimeSpan) currentKeyFrameValue = resolvedKeyFrames[currentKeyFrameIndex].Value;
                else
                {
                    Color fromValue;
                    double currentSegmentProgress;

                    if (currentKeyFrameIndex == 0)
                    {
                        fromValue = from;
                        currentSegmentProgress = currentTime.TotalMilliseconds / resolvedKeyFrames[0].KeyTime.TimeSpan.TotalMilliseconds;
                    }
                    else
                    {
                        int previousKeyFrameIndex = currentKeyFrameIndex - 1;
                        var previousKeyTime = resolvedKeyFrames[previousKeyFrameIndex].KeyTime.TimeSpan;

                        fromValue = resolvedKeyFrames[previousKeyFrameIndex].Value;

                        var segmentCurrentTime = currentTime - previousKeyTime;
                        var segmentDuration = resolvedKeyFrames[currentKeyFrameIndex].KeyTime.TimeSpan - previousKeyTime;

                        currentSegmentProgress = segmentCurrentTime.TotalMilliseconds / segmentDuration.TotalMilliseconds;
                    }

                    currentKeyFrameValue = resolvedKeyFrames[currentKeyFrameIndex].InterpolateValue(fromValue, Math.Min(currentSegmentProgress, 1));
                }

                //=============================================================================================
                if (isCumulative) currentKeyFrameValue += diff * iterationsCount;

                setter(currentKeyFrameValue);
                //=============================================================================================

                frameStopwatch?.Spend();

                if (currentTime >= totalDuration) Finish();
            }

            void Finish()
            {
                if (!finished)
                {
                    finished = true;
                    stopwatch?.Stop();
                    CompositionTarget.Rendering -= Animate;
                    if (name != null)
                    {
                        animation.StateChanged -= StateChanged;
                        animation.End();
                        Animations.Remove(name);
                    }
                    tcs.TrySetResult(null);
                }
            }

            CompositionTarget.Rendering += Animate;

            if (name != null)
            {
                if (Animations.TryGetValue(name, out var oldAnimation))
                {
                    oldAnimation.Stop();
                    await oldAnimation.Wait();
                }
                Animations.Add(name, animation);
            }

            stopwatch?.Start();
            await tcs.Task;
        }

        #endregion

        #region ThicknessAnimation

        public static async Task AnimateThickness(string name, Action<Thickness> setter, Thickness from, Thickness to, TimeSpan duration, RepeatBehavior repeatBehavior = default, bool autoReverse = false, bool isCumulative = false, IEasingFunction easingFunction = null, int? fps = null)
        {
            bool finished = false;
            var tcs = new TaskCompletionSource<object>();
            var animation = new StaticAnimation();
            long iterationsCount = 0;

            double durationMilliseconds = duration.TotalMilliseconds;

            Stopwatch stopwatch = null;
            FrameStopwatch frameStopwatch = null;
            if (fps.HasValue) frameStopwatch = new FrameStopwatch(fps.Value, 0);
            else stopwatch = new Stopwatch();

            double CurrentMilliseconds() => fps.HasValue ? frameStopwatch.ElapsedMilliseconds : stopwatch.ElapsedMilliseconds;

            TimeSpan totalDuration;
            if (repeatBehavior.HasCount) totalDuration = repeatBehavior.Count == 0 ? duration : duration.Multiply(repeatBehavior.Count);
            else if (repeatBehavior.HasDuration) totalDuration = repeatBehavior.Duration;
            else totalDuration = TimeSpan.MaxValue;

            double totalMilliseconds = totalDuration.TotalMilliseconds;

            //=============================================================================================
            double fromLeft = from.Left;
            double fromTop = from.Top;
            double fromRight = from.Right;
            double fromBottom = from.Bottom;

            double diffLeft = to.Left - from.Left;
            double diffTop = to.Top - from.Top;
            double diffRight = to.Right - from.Right;
            double diffBottom = to.Bottom - from.Bottom;
            //=============================================================================================

            if (name != null) animation.StateChanged += StateChanged;

            void StateChanged(object sender, EventArgs<StaticAnimationState> e)
            {
                switch (e.Param1)
                {
                    case StaticAnimationState.Running:
                        stopwatch?.Start();
                        CompositionTarget.Rendering += Animate;
                        break;
                    case StaticAnimationState.Paused:
                        stopwatch?.Stop();
                        CompositionTarget.Rendering -= Animate;
                        break;
                    case StaticAnimationState.Stopped:
                        Finish();
                        break;
                }
            }

            void Animate(object sender, EventArgs e)
            {
                double baseCurrentMilliseconds = CurrentMilliseconds();
                double currentMilliseconds = baseCurrentMilliseconds;
                if (currentMilliseconds > (iterationsCount + 1) * durationMilliseconds && currentMilliseconds < totalMilliseconds) iterationsCount++;

                currentMilliseconds -= durationMilliseconds * iterationsCount;
                if (autoReverse && iterationsCount % 2 != 0) currentMilliseconds = -currentMilliseconds + durationMilliseconds;

                double baseProgress = Math.Min(currentMilliseconds / durationMilliseconds, 1);
                double progress = easingFunction?.Ease(baseProgress) ?? baseProgress;

                //=============================================================================================
                var value = new Thickness(fromLeft + diffLeft * progress, fromTop + diffTop * progress, fromRight + diffRight * progress, fromBottom + diffBottom * progress);

                if (isCumulative)
                {
                    value.Left += diffLeft * iterationsCount;
                    value.Top += diffTop * iterationsCount;
                    value.Right += diffRight * iterationsCount;
                    value.Bottom += diffBottom * iterationsCount;
                }

                setter(value);
                //=============================================================================================

                frameStopwatch?.Spend();

                if (baseCurrentMilliseconds >= totalMilliseconds) Finish();
            }

            void Finish()
            {
                if (!finished)
                {
                    finished = true;
                    stopwatch?.Stop();
                    CompositionTarget.Rendering -= Animate;
                    if (name != null)
                    {
                        animation.StateChanged -= StateChanged;
                        Animations.Remove(name);
                        animation.End();
                    }
                    tcs.TrySetResult(null);
                }
            }

            CompositionTarget.Rendering += Animate;

            if (name != null)
            {
                if (Animations.TryGetValue(name, out var oldAnimation))
                {
                    oldAnimation.Stop();
                    await oldAnimation.Wait();
                }
                Animations.Add(name, animation);
            }

            stopwatch?.Start();
            await tcs.Task;
        }

        public static async Task AnimateThicknessArray(string name, Action<Thickness[]> setter, IList<Thickness> from, IList<Thickness> to, TimeSpan duration, RepeatBehavior repeatBehavior = default, bool autoReverse = false, bool isCumulative = false, IEasingFunction easingFunction = null, int? fps = null)
        {
            bool finished = false;
            var tcs = new TaskCompletionSource<object>();
            var animation = new StaticAnimation();
            long iterationsCount = 0;

            double durationMilliseconds = duration.TotalMilliseconds;

            Stopwatch stopwatch = null;
            FrameStopwatch frameStopwatch = null;
            if (fps.HasValue) frameStopwatch = new FrameStopwatch(fps.Value, 0);
            else stopwatch = new Stopwatch();

            double CurrentMilliseconds() => fps.HasValue ? frameStopwatch.ElapsedMilliseconds : stopwatch.ElapsedMilliseconds;

            TimeSpan totalDuration;
            if (repeatBehavior.HasCount) totalDuration = repeatBehavior.Count == 0 ? duration : duration.Multiply(repeatBehavior.Count);
            else if (repeatBehavior.HasDuration) totalDuration = repeatBehavior.Duration;
            else totalDuration = TimeSpan.MaxValue;

            double totalMilliseconds = totalDuration.TotalMilliseconds;

            //=============================================================================================
            int count = from.Count;
            var diff = new Thickness[count];

            for (int i = 0; i < count; i++) diff[i] = new Thickness(to[i].Left - from[i].Left, to[i].Top - from[i].Top, to[i].Right - from[i].Right, to[i].Bottom - from[i].Bottom);
            //=============================================================================================

            if (name != null) animation.StateChanged += StateChanged;

            void StateChanged(object sender, EventArgs<StaticAnimationState> e)
            {
                switch (e.Param1)
                {
                    case StaticAnimationState.Running:
                        stopwatch?.Start();
                        CompositionTarget.Rendering += Animate;
                        break;
                    case StaticAnimationState.Paused:
                        stopwatch?.Stop();
                        CompositionTarget.Rendering -= Animate;
                        break;
                    case StaticAnimationState.Stopped:
                        Finish();
                        break;
                }
            }

            void Animate(object sender, EventArgs e)
            {
                double baseCurrentMilliseconds = CurrentMilliseconds();
                double currentMilliseconds = baseCurrentMilliseconds;
                if (currentMilliseconds > (iterationsCount + 1) * durationMilliseconds && currentMilliseconds < totalMilliseconds) iterationsCount++;

                currentMilliseconds -= durationMilliseconds * iterationsCount;
                if (autoReverse && iterationsCount % 2 != 0) currentMilliseconds = -currentMilliseconds + durationMilliseconds;

                double baseProgress = Math.Min(currentMilliseconds / durationMilliseconds, 1);
                double progress = easingFunction?.Ease(baseProgress) ?? baseProgress;

                //=============================================================================================
                var value = new Thickness[count];

                for (int i = 0; i < count; i++)
                {
                    var dif = diff[i];
                    var val = new Thickness(from[i].Left + diff[i].Left * progress, from[i].Top + diff[i].Top * progress, from[i].Right + diff[i].Right * progress, from[i].Bottom + diff[i].Bottom * progress);
                    if (isCumulative)
                    {
                        val.Left += dif.Left * iterationsCount;
                        val.Top += dif.Top * iterationsCount;
                        val.Right += dif.Right * iterationsCount;
                        val.Bottom += dif.Bottom * iterationsCount;
                    }

                    value[i] = val;
                }

                setter(value);
                //=============================================================================================

                frameStopwatch?.Spend();

                if (baseCurrentMilliseconds >= totalMilliseconds) Finish();
            }

            void Finish()
            {
                if (!finished)
                {
                    finished = true;
                    stopwatch?.Stop();
                    CompositionTarget.Rendering -= Animate;
                    if (name != null)
                    {
                        animation.StateChanged -= StateChanged;
                        Animations.Remove(name);
                        animation.End();
                    }
                    tcs.TrySetResult(null);
                }
            }

            CompositionTarget.Rendering += Animate;

            if (name != null)
            {
                if (Animations.TryGetValue(name, out var oldAnimation))
                {
                    oldAnimation.Stop();
                    await oldAnimation.Wait();
                }
                Animations.Add(name, animation);
            }

            stopwatch?.Start();
            await tcs.Task;
        }

        public static async Task AnimateThicknessUsingKeyFrames(string name, Action<Thickness> setter, Thickness from, IEnumerable<ThicknessKeyFrame> keyFrames, Duration duration, RepeatBehavior repeatBehavior = default, bool autoReverse = false, bool isCumulative = false, int? fps = null)
        {
            bool finished = false;
            var tcs = new TaskCompletionSource<object>();
            var animation = new StaticAnimation();
            long iterationsCount = 0;

            Stopwatch stopwatch = null;
            FrameStopwatch frameStopwatch = null;
            if (fps.HasValue) frameStopwatch = new FrameStopwatch(fps.Value, 0);
            else stopwatch = new Stopwatch();

            TimeSpan CurrentTime() => fps.HasValue ? frameStopwatch.Elapsed : stopwatch.Elapsed;

            var resolvedKeyFrames = ResolveKeyTimes(keyFrames, duration, out var durationTime);

            //=============================================================================================
            var firstValue = resolvedKeyFrames[0].Value;
            var lastValue = resolvedKeyFrames[resolvedKeyFrames.Length - 1].Value;
            var diff = new Thickness(lastValue.Left - firstValue.Left, lastValue.Top - firstValue.Top, lastValue.Right - firstValue.Right, lastValue.Bottom - firstValue.Bottom);
            //=============================================================================================

            TimeSpan totalDuration;
            if (repeatBehavior.HasCount) totalDuration = repeatBehavior.Count == 0 ? durationTime : durationTime.Multiply(repeatBehavior.Count);
            else if (repeatBehavior.HasDuration) totalDuration = repeatBehavior.Duration;
            else totalDuration = TimeSpan.MaxValue;

            if (name != null) animation.StateChanged += StateChanged;

            void StateChanged(object sender, EventArgs<StaticAnimationState> e)
            {
                switch (e.Param1)
                {
                    case StaticAnimationState.Running:
                        stopwatch?.Start();
                        CompositionTarget.Rendering += Animate;
                        break;
                    case StaticAnimationState.Paused:
                        stopwatch?.Stop();
                        CompositionTarget.Rendering -= Animate;
                        break;
                    case StaticAnimationState.Stopped:
                        Finish();
                        break;
                }
            }

            void Animate(object sender, EventArgs e)
            {
                var currentTime = CurrentTime();
                if (currentTime > durationTime.Multiply(iterationsCount + 1) && currentTime < totalDuration) iterationsCount++;

                currentTime -= durationTime.Multiply(iterationsCount);
                if (autoReverse && iterationsCount % 2 != 0) currentTime = -currentTime + durationTime;

                int maxKeyFrameIndex = resolvedKeyFrames.Length - 1;
                int currentKeyFrameIndex = 0;
                Thickness currentKeyFrameValue;

                while (currentKeyFrameIndex < resolvedKeyFrames.Length && currentTime > resolvedKeyFrames[currentKeyFrameIndex].KeyTime.TimeSpan) currentKeyFrameIndex++;
                while (currentKeyFrameIndex < maxKeyFrameIndex && currentTime == resolvedKeyFrames[currentKeyFrameIndex + 1].KeyTime.TimeSpan) currentKeyFrameIndex++;

                if (currentKeyFrameIndex == resolvedKeyFrames.Length) currentKeyFrameValue = resolvedKeyFrames[maxKeyFrameIndex].Value;
                else if (currentTime == resolvedKeyFrames[currentKeyFrameIndex].KeyTime.TimeSpan) currentKeyFrameValue = resolvedKeyFrames[currentKeyFrameIndex].Value;
                else
                {
                    Thickness fromValue;
                    double currentSegmentProgress;

                    if (currentKeyFrameIndex == 0)
                    {
                        fromValue = from;
                        currentSegmentProgress = currentTime.TotalMilliseconds / resolvedKeyFrames[0].KeyTime.TimeSpan.TotalMilliseconds;
                    }
                    else
                    {
                        int previousKeyFrameIndex = currentKeyFrameIndex - 1;
                        var previousKeyTime = resolvedKeyFrames[previousKeyFrameIndex].KeyTime.TimeSpan;

                        fromValue = resolvedKeyFrames[previousKeyFrameIndex].Value;

                        var segmentCurrentTime = currentTime - previousKeyTime;
                        var segmentDuration = resolvedKeyFrames[currentKeyFrameIndex].KeyTime.TimeSpan - previousKeyTime;

                        currentSegmentProgress = segmentCurrentTime.TotalMilliseconds / segmentDuration.TotalMilliseconds;
                    }

                    currentKeyFrameValue = resolvedKeyFrames[currentKeyFrameIndex].InterpolateValue(fromValue, Math.Min(currentSegmentProgress, 1));
                }

                //=============================================================================================
                if (isCumulative)
                {
                    currentKeyFrameValue.Left += diff.Left * iterationsCount;
                    currentKeyFrameValue.Top += diff.Top * iterationsCount;
                    currentKeyFrameValue.Right += diff.Right * iterationsCount;
                    currentKeyFrameValue.Bottom += diff.Bottom * iterationsCount;
                }

                setter(currentKeyFrameValue);
                //=============================================================================================

                frameStopwatch?.Spend();

                if (currentTime >= totalDuration) Finish();
            }

            void Finish()
            {
                if (!finished)
                {
                    finished = true;
                    stopwatch?.Stop();
                    CompositionTarget.Rendering -= Animate;
                    if (name != null)
                    {
                        animation.StateChanged -= StateChanged;
                        animation.End();
                        Animations.Remove(name);
                    }
                    tcs.TrySetResult(null);
                }
            }

            CompositionTarget.Rendering += Animate;

            if (name != null)
            {
                if (Animations.TryGetValue(name, out var oldAnimation))
                {
                    oldAnimation.Stop();
                    await oldAnimation.Wait();
                }
                Animations.Add(name, animation);
            }

            stopwatch?.Start();
            await tcs.Task;
        }

        #endregion

        #region SizeAnimation

        public static async Task AnimateSize(string name, Action<Size> setter, Size from, Size to, TimeSpan duration, RepeatBehavior repeatBehavior = default, bool autoReverse = false, bool isCumulative = false, IEasingFunction easingFunction = null, int? fps = null)
        {
            bool finished = false;
            var tcs = new TaskCompletionSource<object>();
            var animation = new StaticAnimation();
            long iterationsCount = 0;

            double durationMilliseconds = duration.TotalMilliseconds;

            Stopwatch stopwatch = null;
            FrameStopwatch frameStopwatch = null;
            if (fps.HasValue) frameStopwatch = new FrameStopwatch(fps.Value, 0);
            else stopwatch = new Stopwatch();

            double CurrentMilliseconds() => fps.HasValue ? frameStopwatch.ElapsedMilliseconds : stopwatch.ElapsedMilliseconds;

            TimeSpan totalDuration;
            if (repeatBehavior.HasCount) totalDuration = repeatBehavior.Count == 0 ? duration : duration.Multiply(repeatBehavior.Count);
            else if (repeatBehavior.HasDuration) totalDuration = repeatBehavior.Duration;
            else totalDuration = TimeSpan.MaxValue;

            double totalMilliseconds = totalDuration.TotalMilliseconds;

            //=============================================================================================
            double fromWidth = from.Width;
            double fromHeight = from.Height;

            double diffWidth = to.Width - from.Width;
            double diffHeight = to.Height - from.Height;
            //=============================================================================================

            if (name != null) animation.StateChanged += StateChanged;

            void StateChanged(object sender, EventArgs<StaticAnimationState> e)
            {
                switch (e.Param1)
                {
                    case StaticAnimationState.Running:
                        stopwatch?.Start();
                        CompositionTarget.Rendering += Animate;
                        break;
                    case StaticAnimationState.Paused:
                        stopwatch?.Stop();
                        CompositionTarget.Rendering -= Animate;
                        break;
                    case StaticAnimationState.Stopped:
                        Finish();
                        break;
                }
            }

            void Animate(object sender, EventArgs e)
            {
                double baseCurrentMilliseconds = CurrentMilliseconds();
                double currentMilliseconds = baseCurrentMilliseconds;
                if (currentMilliseconds > (iterationsCount + 1) * durationMilliseconds && currentMilliseconds < totalMilliseconds) iterationsCount++;

                currentMilliseconds -= durationMilliseconds * iterationsCount;
                if (autoReverse && iterationsCount % 2 != 0) currentMilliseconds = -currentMilliseconds + durationMilliseconds;

                double baseProgress = Math.Min(currentMilliseconds / durationMilliseconds, 1);
                double progress = easingFunction?.Ease(baseProgress) ?? baseProgress;

                //=============================================================================================
                var value = new Size(fromWidth + diffWidth * progress, fromHeight + diffHeight * progress);

                if (isCumulative)
                {
                    value.Width += diffWidth * iterationsCount;
                    value.Height += diffHeight * iterationsCount;
                }

                setter(value);
                //=============================================================================================

                frameStopwatch?.Spend();

                if (baseCurrentMilliseconds >= totalMilliseconds) Finish();
            }

            void Finish()
            {
                if (!finished)
                {
                    finished = true;
                    stopwatch?.Stop();
                    CompositionTarget.Rendering -= Animate;
                    if (name != null)
                    {
                        animation.StateChanged -= StateChanged;
                        Animations.Remove(name);
                        animation.End();
                    }
                    tcs.TrySetResult(null);
                }
            }

            CompositionTarget.Rendering += Animate;

            if (name != null)
            {
                if (Animations.TryGetValue(name, out var oldAnimation))
                {
                    oldAnimation.Stop();
                    await oldAnimation.Wait();
                }
                Animations.Add(name, animation);
            }

            stopwatch?.Start();
            await tcs.Task;
        }

        public static async Task AnimateSizeArray(string name, Action<Size[]> setter, IList<Size> from, IList<Size> to, TimeSpan duration, RepeatBehavior repeatBehavior = default, bool autoReverse = false, bool isCumulative = false, IEasingFunction easingFunction = null, int? fps = null)
        {
            bool finished = false;
            var tcs = new TaskCompletionSource<object>();
            var animation = new StaticAnimation();
            long iterationsCount = 0;

            double durationMilliseconds = duration.TotalMilliseconds;

            Stopwatch stopwatch = null;
            FrameStopwatch frameStopwatch = null;
            if (fps.HasValue) frameStopwatch = new FrameStopwatch(fps.Value, 0);
            else stopwatch = new Stopwatch();

            double CurrentMilliseconds() => fps.HasValue ? frameStopwatch.ElapsedMilliseconds : stopwatch.ElapsedMilliseconds;

            TimeSpan totalDuration;
            if (repeatBehavior.HasCount) totalDuration = repeatBehavior.Count == 0 ? duration : duration.Multiply(repeatBehavior.Count);
            else if (repeatBehavior.HasDuration) totalDuration = repeatBehavior.Duration;
            else totalDuration = TimeSpan.MaxValue;

            double totalMilliseconds = totalDuration.TotalMilliseconds;

            //=============================================================================================
            int count = from.Count;
            var diff = new Size[count];

            for (int i = 0; i < count; i++) diff[i] = new Size(to[i].Width - from[i].Width, to[i].Height - from[i].Height);
            //=============================================================================================

            if (name != null) animation.StateChanged += StateChanged;

            void StateChanged(object sender, EventArgs<StaticAnimationState> e)
            {
                switch (e.Param1)
                {
                    case StaticAnimationState.Running:
                        stopwatch?.Start();
                        CompositionTarget.Rendering += Animate;
                        break;
                    case StaticAnimationState.Paused:
                        stopwatch?.Stop();
                        CompositionTarget.Rendering -= Animate;
                        break;
                    case StaticAnimationState.Stopped:
                        Finish();
                        break;
                }
            }

            void Animate(object sender, EventArgs e)
            {
                double baseCurrentMilliseconds = CurrentMilliseconds();
                double currentMilliseconds = baseCurrentMilliseconds;
                if (currentMilliseconds > (iterationsCount + 1) * durationMilliseconds && currentMilliseconds < totalMilliseconds) iterationsCount++;

                currentMilliseconds -= durationMilliseconds * iterationsCount;
                if (autoReverse && iterationsCount % 2 != 0) currentMilliseconds = -currentMilliseconds + durationMilliseconds;

                double baseProgress = Math.Min(currentMilliseconds / durationMilliseconds, 1);
                double progress = easingFunction?.Ease(baseProgress) ?? baseProgress;

                //=============================================================================================
                var value = new Size[count];

                for (int i = 0; i < count; i++)
                {
                    var dif = diff[i];
                    var val = new Size(from[i].Width + diff[i].Width * progress, from[i].Height + diff[i].Height * progress);
                    if (isCumulative)
                    {
                        val.Width += dif.Width * iterationsCount;
                        val.Height += dif.Height * iterationsCount;
                    }

                    value[i] = val;
                }

                setter(value);
                //=============================================================================================

                frameStopwatch?.Spend();

                if (baseCurrentMilliseconds >= totalMilliseconds) Finish();
            }

            void Finish()
            {
                if (!finished)
                {
                    finished = true;
                    stopwatch?.Stop();
                    CompositionTarget.Rendering -= Animate;
                    if (name != null)
                    {
                        animation.StateChanged -= StateChanged;
                        Animations.Remove(name);
                        animation.End();
                    }
                    tcs.TrySetResult(null);
                }
            }

            CompositionTarget.Rendering += Animate;

            if (name != null)
            {
                if (Animations.TryGetValue(name, out var oldAnimation))
                {
                    oldAnimation.Stop();
                    await oldAnimation.Wait();
                }
                Animations.Add(name, animation);
            }

            stopwatch?.Start();
            await tcs.Task;
        }

        public static async Task AnimateSizeUsingKeyFrames(string name, Action<Size> setter, Size from, IEnumerable<SizeKeyFrame> keyFrames, Duration duration, RepeatBehavior repeatBehavior = default, bool autoReverse = false, bool isCumulative = false, int? fps = null)
        {
            bool finished = false;
            var tcs = new TaskCompletionSource<object>();
            var animation = new StaticAnimation();
            long iterationsCount = 0;

            Stopwatch stopwatch = null;
            FrameStopwatch frameStopwatch = null;
            if (fps.HasValue) frameStopwatch = new FrameStopwatch(fps.Value, 0);
            else stopwatch = new Stopwatch();

            TimeSpan CurrentTime() => fps.HasValue ? frameStopwatch.Elapsed : stopwatch.Elapsed;

            var resolvedKeyFrames = ResolveKeyTimes(keyFrames, duration, out var durationTime);

            //=============================================================================================
            var firstValue = resolvedKeyFrames[0].Value;
            var lastValue = resolvedKeyFrames[resolvedKeyFrames.Length - 1].Value;
            var diff = new Size(lastValue.Width - firstValue.Width, lastValue.Height - firstValue.Height);
            //=============================================================================================

            TimeSpan totalDuration;
            if (repeatBehavior.HasCount) totalDuration = repeatBehavior.Count == 0 ? durationTime : durationTime.Multiply(repeatBehavior.Count);
            else if (repeatBehavior.HasDuration) totalDuration = repeatBehavior.Duration;
            else totalDuration = TimeSpan.MaxValue;

            if (name != null) animation.StateChanged += StateChanged;

            void StateChanged(object sender, EventArgs<StaticAnimationState> e)
            {
                switch (e.Param1)
                {
                    case StaticAnimationState.Running:
                        stopwatch?.Start();
                        CompositionTarget.Rendering += Animate;
                        break;
                    case StaticAnimationState.Paused:
                        stopwatch?.Stop();
                        CompositionTarget.Rendering -= Animate;
                        break;
                    case StaticAnimationState.Stopped:
                        Finish();
                        break;
                }
            }

            void Animate(object sender, EventArgs e)
            {
                var currentTime = CurrentTime();
                if (currentTime > durationTime.Multiply(iterationsCount + 1) && currentTime < totalDuration) iterationsCount++;

                currentTime -= durationTime.Multiply(iterationsCount);
                if (autoReverse && iterationsCount % 2 != 0) currentTime = -currentTime + durationTime;

                int maxKeyFrameIndex = resolvedKeyFrames.Length - 1;
                int currentKeyFrameIndex = 0;
                Size currentKeyFrameValue;

                while (currentKeyFrameIndex < resolvedKeyFrames.Length && currentTime > resolvedKeyFrames[currentKeyFrameIndex].KeyTime.TimeSpan) currentKeyFrameIndex++;
                while (currentKeyFrameIndex < maxKeyFrameIndex && currentTime == resolvedKeyFrames[currentKeyFrameIndex + 1].KeyTime.TimeSpan) currentKeyFrameIndex++;

                if (currentKeyFrameIndex == resolvedKeyFrames.Length) currentKeyFrameValue = resolvedKeyFrames[maxKeyFrameIndex].Value;
                else if (currentTime == resolvedKeyFrames[currentKeyFrameIndex].KeyTime.TimeSpan) currentKeyFrameValue = resolvedKeyFrames[currentKeyFrameIndex].Value;
                else
                {
                    Size fromValue;
                    double currentSegmentProgress;

                    if (currentKeyFrameIndex == 0)
                    {
                        fromValue = from;
                        currentSegmentProgress = currentTime.TotalMilliseconds / resolvedKeyFrames[0].KeyTime.TimeSpan.TotalMilliseconds;
                    }
                    else
                    {
                        int previousKeyFrameIndex = currentKeyFrameIndex - 1;
                        var previousKeyTime = resolvedKeyFrames[previousKeyFrameIndex].KeyTime.TimeSpan;

                        fromValue = resolvedKeyFrames[previousKeyFrameIndex].Value;

                        var segmentCurrentTime = currentTime - previousKeyTime;
                        var segmentDuration = resolvedKeyFrames[currentKeyFrameIndex].KeyTime.TimeSpan - previousKeyTime;

                        currentSegmentProgress = segmentCurrentTime.TotalMilliseconds / segmentDuration.TotalMilliseconds;
                    }

                    currentKeyFrameValue = resolvedKeyFrames[currentKeyFrameIndex].InterpolateValue(fromValue, Math.Min(currentSegmentProgress, 1));
                }

                //=============================================================================================
                if (isCumulative)
                {
                    currentKeyFrameValue.Width += diff.Width * iterationsCount;
                    currentKeyFrameValue.Height += diff.Height * iterationsCount;
                }

                setter(currentKeyFrameValue);
                //=============================================================================================

                frameStopwatch?.Spend();

                if (currentTime >= totalDuration) Finish();
            }

            void Finish()
            {
                if (!finished)
                {
                    finished = true;
                    stopwatch?.Stop();
                    CompositionTarget.Rendering -= Animate;
                    if (name != null)
                    {
                        animation.StateChanged -= StateChanged;
                        animation.End();
                        Animations.Remove(name);
                    }
                    tcs.TrySetResult(null);
                }
            }

            CompositionTarget.Rendering += Animate;

            if (name != null)
            {
                if (Animations.TryGetValue(name, out var oldAnimation))
                {
                    oldAnimation.Stop();
                    await oldAnimation.Wait();
                }
                Animations.Add(name, animation);
            }

            stopwatch?.Start();
            await tcs.Task;
        }

        #endregion

        private struct KeyTimeBlock
        {
            public int BeginIndex { get; set; }
            public int EndIndex { get; set; }
        }

        private struct ResolvedKeyFrame
        {
            public int OriginalIndex { get; set; }
            public TimeSpan KeyTime { get; set; }
        }

        public static T[] ResolveKeyTimes<T>(IEnumerable<T> keyFrames, Duration duration, out TimeSpan durationTime) where T : IKeyFrame
        {
            var keyFramesArray = keyFrames.Where(keyFrame => keyFrame.KeyTime.Type != KeyTimeType.Paced).ToArray();
            if (keyFramesArray.IsNullOrEmpty())
            {
                durationTime = default;
                return keyFramesArray;
            }

            var resolvedKeyFrames = new ResolvedKeyFrame[keyFramesArray.Length];

            for (int i = 0; i < resolvedKeyFrames.Length; i++) resolvedKeyFrames[i].OriginalIndex = i;

            TimeSpan LargestTimeSpanKeyTime()
            {
                bool hasTimeSpanKeyTime = false;
                var largestTimeSpanKeyTime = TimeSpan.Zero;

                for (int i = 0; i < keyFramesArray.Length; i++)
                {
                    var keyTime = keyFramesArray[i].KeyTime;

                    if (keyTime.Type == KeyTimeType.TimeSpan)
                    {
                        hasTimeSpanKeyTime = true;

                        if (keyTime.TimeSpan > largestTimeSpanKeyTime)
                        {
                            largestTimeSpanKeyTime = keyTime.TimeSpan;
                        }
                    }
                }

                if (hasTimeSpanKeyTime) return largestTimeSpanKeyTime;
                else return TimeSpan.FromSeconds(1.0);
            }

            durationTime = TimeSpan.FromSeconds(1);

            if (duration.HasTimeSpan) durationTime = duration.TimeSpan;
            else durationTime = LargestTimeSpanKeyTime();

            int maxKeyFrameIndex = keyFramesArray.Length - 1;
            var unspecifiedBlocks = new ArrayList();
            //bool hasPacedKeyTimes = false;

            //
            // Pass 1: Resolve Percent and Time key times.
            //

            for (int i = 0; i < keyFramesArray.Length; i++)
            {
                var keyTime = keyFramesArray[i].KeyTime;

                switch (keyTime.Type)
                {
                    case KeyTimeType.Percent:
                        resolvedKeyFrames[i].KeyTime = TimeSpan.FromMilliseconds(keyTime.Percent * durationTime.TotalMilliseconds);
                        break;

                    case KeyTimeType.TimeSpan:
                        resolvedKeyFrames[i].KeyTime = keyTime.TimeSpan;
                        break;

                    //case KeyTimeType.Paced:
                    case KeyTimeType.Uniform:

                        if (i == maxKeyFrameIndex) resolvedKeyFrames[i].KeyTime = durationTime;
                        //else if (i == 0 && keyTime.Type == KeyTimeType.Paced) sortedKeyFrames[i].KeyTime = TimeSpan.Zero;
                        else
                        {
                            //if (keyTime.Type == KeyTimeType.Paced) hasPacedKeyTimes = true;

                            var block = new KeyTimeBlock { BeginIndex = i };

                            while (++i < maxKeyFrameIndex)
                            {
                                var type = keyFramesArray[i].KeyTime.Type;

                                if (type == KeyTimeType.Percent || type == KeyTimeType.TimeSpan) break;
                                //else if (type == KeyTimeType.Paced) hasPacedKeyTimes = true;
                            }

                            block.EndIndex = i;
                            unspecifiedBlocks.Add(block);
                            i--;
                        }

                        break;
                }
            }

            //
            // Pass 2: Resolve Uniform key times.
            //

            for (int i = 0; i < unspecifiedBlocks.Count; i++)
            {
                var block = (KeyTimeBlock)unspecifiedBlocks[i];

                var blockBeginTime = TimeSpan.Zero;

                if (block.BeginIndex > 0) blockBeginTime = resolvedKeyFrames[block.BeginIndex - 1].KeyTime;

                long segmentCount = block.EndIndex - block.BeginIndex + 1;
                var uniformTimeStep = TimeSpan.FromTicks((resolvedKeyFrames[block.EndIndex].KeyTime - blockBeginTime).Ticks / segmentCount);

                var resolvedTime = blockBeginTime + uniformTimeStep;

                for (int j = block.BeginIndex; j < block.EndIndex; j++)
                {
                    resolvedKeyFrames[j].KeyTime = resolvedTime;
                    resolvedTime += uniformTimeStep;
                }
            }

            return resolvedKeyFrames.OrderBy(resolvedKeyFrame => resolvedKeyFrame.KeyTime).Select(resolvedKeyFrame =>
            {
                var keyFrame = keyFramesArray[resolvedKeyFrame.OriginalIndex];
                keyFrame.KeyTime = resolvedKeyFrame.KeyTime;
                return keyFrame;
            }).ToArray();
        }
    }

    public class StaticAnimation
    {
        private StaticAnimationState m_state;
        public StaticAnimationState State
        {
            get => m_state;
            private set
            {
                if (m_state != value)
                {
                    m_state = value;
                    StateChanged?.Invoke(this, EventArgsHelper.Create(m_state));
                }
            }
        }

        public event EventHandler<EventArgs<StaticAnimationState>> StateChanged;
        public event EventHandler Ended;

        public bool IsEnded { get; private set; }

        internal void End()
        {
            if (!IsEnded)
            {
                IsEnded = true;
                Ended?.Invoke(this, EventArgs.Empty);
            }
        }

        public void Start() => State = StaticAnimationState.Running;
        public void Pause() => State = StaticAnimationState.Paused;
        public void Stop() => State = StaticAnimationState.Stopped;

        public Task Wait()
        {
            if (!IsEnded)
            {
                var tcs = new TaskCompletionSource<object>();

                Ended += OnEnded;

                void OnEnded(object sender, EventArgs e)
                {
                    Ended -= OnEnded;
                    tcs.TrySetResult(null);
                }

                return tcs.Task;
            }
            else return Task.CompletedTask;
        }
    }

    public enum StaticAnimationState { Running, Paused, Stopped }

    public class SmoothEase : IEasingFunction
    {
        public SmoothEase() => Inflection = 10;
        public SmoothEase(int inflection) => Inflection = inflection;

        public int Inflection { get; set; }

        public double Ease(double normalizedTime) => EaseOn(normalizedTime, Inflection);

        public static double EaseOn(double normalizedTime, double inflection = 10)
        {
            double error = Standard.Num.Sigmoid(-inflection / 2);
            return (Standard.Num.Sigmoid(inflection * (normalizedTime - 0.5)) - error) / (1 - 2 * error);
        }
    }

    public class DoubleSmoothEase : IEasingFunction
    {
        public DoubleSmoothEase() => Inflection = 10;
        public DoubleSmoothEase(int inflection) => Inflection = inflection;

        public int Inflection { get; set; }

        public double Ease(double normalizedTime) => EaseOn(normalizedTime, Inflection);

        public static double EaseOn(double normalizedTime, double inflection = 10) => normalizedTime < 0.5 ? 0.5 * SmoothEase.EaseOn(2 * normalizedTime, inflection) : 0.5 * (1 + SmoothEase.EaseOn(2 * normalizedTime - 1, inflection));
    }
}
