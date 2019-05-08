namespace System
{
    public static class EventArgsHelper
    {
        public static EventArgs<T1> Create<T1>(T1 param1) => new EventArgs<T1>(param1);
        public static EventArgs<T1, T2> Create<T1, T2>(T1 param1, T2 param2) => new EventArgs<T1, T2>(param1, param2);
        public static EventArgs<T1, T2, T3> Create<T1, T2, T3>(T1 param1, T2 param2, T3 param3) => new EventArgs<T1, T2, T3>(param1, param2, param3);
        public static EventArgs<T1, T2, T3, T4> Create<T1, T2, T3, T4>(T1 param1, T2 param2, T3 param3, T4 param4) => new EventArgs<T1, T2, T3, T4>(param1, param2, param3, param4);
        public static EventArgs<T1, T2, T3, T4, T5> Create<T1, T2, T3, T4, T5>(T1 param1, T2 param2, T3 param3, T4 param4, T5 param5) => new EventArgs<T1, T2, T3, T4, T5>(param1, param2, param3, param4, param5);
        public static EventArgs<T1, T2, T3, T4, T5, T6> Create<T1, T2, T3, T4, T5, T6>(T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6) => new EventArgs<T1, T2, T3, T4, T5, T6>(param1, param2, param3, param4, param5, param6);
        public static EventArgs<T1, T2, T3, T4, T5, T6, T7> Create<T1, T2, T3, T4, T5, T6, T7>(T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7) => new EventArgs<T1, T2, T3, T4, T5, T6, T7>(param1, param2, param3, param4, param5, param6, param7);
        public static EventArgs<T1, T2, T3, T4, T5, T6, T7, T8> Create<T1, T2, T3, T4, T5, T6, T7, T8>(T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8) => new EventArgs<T1, T2, T3, T4, T5, T6, T7, T8>(param1, param2, param3, param4, param5, param6, param7, param8);
        public static EventArgs<T1, T2, T3, T4, T5, T6, T7, T8, T9> Create<T1, T2, T3, T4, T5, T6, T7, T8, T9>(T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8, T9 param9) => new EventArgs<T1, T2, T3, T4, T5, T6, T7, T8, T9>(param1, param2, param3, param4, param5, param6, param7, param8, param9);
        public static EventArgs<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> Create<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8, T9 param9, T10 param10) => new EventArgs<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(param1, param2, param3, param4, param5, param6, param7, param8, param9, param10);
        public static EventArgs<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> Create<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8, T9 param9, T10 param10, T11 param11) => new EventArgs<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(param1, param2, param3, param4, param5, param6, param7, param8, param9, param10, param11);
        public static EventArgs<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> Create<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8, T9 param9, T10 param10, T11 param11, T12 param12) => new EventArgs<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(param1, param2, param3, param4, param5, param6, param7, param8, param9, param10, param11, param12);
        public static EventArgs<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> Create<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8, T9 param9, T10 param10, T11 param11, T12 param12, T13 param13) => new EventArgs<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(param1, param2, param3, param4, param5, param6, param7, param8, param9, param10, param11, param12, param13);
        public static EventArgs<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> Create<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8, T9 param9, T10 param10, T11 param11, T12 param12, T13 param13, T14 param14) => new EventArgs<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(param1, param2, param3, param4, param5, param6, param7, param8, param9, param10, param11, param12, param13, param14);
        public static EventArgs<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> Create<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8, T9 param9, T10 param10, T11 param11, T12 param12, T13 param13, T14 param14, T15 param15) => new EventArgs<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(param1, param2, param3, param4, param5, param6, param7, param8, param9, param10, param11, param12, param13, param14, param15);
        public static EventArgs<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16> Create<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8, T9 param9, T10 param10, T11 param11, T12 param12, T13 param13, T14 param14, T15 param15, T16 param16) => new EventArgs<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(param1, param2, param3, param4, param5, param6, param7, param8, param9, param10, param11, param12, param13, param14, param15, param16);
    }

    public class EventArgs<T1> : EventArgs
    {
        public EventArgs(T1 param1) => Param1 = param1;

        public T1 Param1 { get; set; }
    }

    public class EventArgs<T1, T2> : EventArgs
    {
        public EventArgs(T1 param1, T2 param2)
        {
            Param1 = param1;
            Param2 = param2;
        }

        public T1 Param1 { get; set; }
        public T2 Param2 { get; set; }
    }

    public class EventArgs<T1, T2, T3> : EventArgs
    {
        public EventArgs(T1 param1, T2 param2, T3 param3)
        {
            Param1 = param1;
            Param2 = param2;
            Param3 = param3;
        }

        public T1 Param1 { get; set; }
        public T2 Param2 { get; set; }
        public T3 Param3 { get; set; }
    }

    public class EventArgs<T1, T2, T3, T4> : EventArgs
    {
        public EventArgs(T1 param1, T2 param2, T3 param3, T4 param4)
        {
            Param1 = param1;
            Param2 = param2;
            Param3 = param3;
            Param4 = param4;
        }

        public T1 Param1 { get; set; }
        public T2 Param2 { get; set; }
        public T3 Param3 { get; set; }
        public T4 Param4 { get; set; }
    }

    public class EventArgs<T1, T2, T3, T4, T5> : EventArgs
    {
        public EventArgs(T1 param1, T2 param2, T3 param3, T4 param4, T5 param5)
        {
            Param1 = param1;
            Param2 = param2;
            Param3 = param3;
            Param4 = param4;
            Param5 = param5;
        }

        public T1 Param1 { get; set; }
        public T2 Param2 { get; set; }
        public T3 Param3 { get; set; }
        public T4 Param4 { get; set; }
        public T5 Param5 { get; set; }
    }

    public class EventArgs<T1, T2, T3, T4, T5, T6> : EventArgs
    {
        public EventArgs(T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6)
        {
            Param1 = param1;
            Param2 = param2;
            Param3 = param3;
            Param4 = param4;
            Param5 = param5;
            Param6 = param6;
        }

        public T1 Param1 { get; set; }
        public T2 Param2 { get; set; }
        public T3 Param3 { get; set; }
        public T4 Param4 { get; set; }
        public T5 Param5 { get; set; }
        public T6 Param6 { get; set; }
    }

    public class EventArgs<T1, T2, T3, T4, T5, T6, T7> : EventArgs
    {
        public EventArgs(T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7)
        {
            Param1 = param1;
            Param2 = param2;
            Param3 = param3;
            Param4 = param4;
            Param5 = param5;
            Param6 = param6;
            Param7 = param7;
        }

        public T1 Param1 { get; set; }
        public T2 Param2 { get; set; }
        public T3 Param3 { get; set; }
        public T4 Param4 { get; set; }
        public T5 Param5 { get; set; }
        public T6 Param6 { get; set; }
        public T7 Param7 { get; set; }
    }

    public class EventArgs<T1, T2, T3, T4, T5, T6, T7, T8> : EventArgs
    {
        public EventArgs(T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8)
        {
            Param1 = param1;
            Param2 = param2;
            Param3 = param3;
            Param4 = param4;
            Param5 = param5;
            Param6 = param6;
            Param7 = param7;
            Param8 = param8;
        }

        public T1 Param1 { get; set; }
        public T2 Param2 { get; set; }
        public T3 Param3 { get; set; }
        public T4 Param4 { get; set; }
        public T5 Param5 { get; set; }
        public T6 Param6 { get; set; }
        public T7 Param7 { get; set; }
        public T8 Param8 { get; set; }
    }

    public class EventArgs<T1, T2, T3, T4, T5, T6, T7, T8, T9> : EventArgs
    {
        public EventArgs(T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8, T9 param9)
        {
            Param1 = param1;
            Param2 = param2;
            Param3 = param3;
            Param4 = param4;
            Param5 = param5;
            Param6 = param6;
            Param7 = param7;
            Param8 = param8;
            Param9 = param9;
        }

        public T1 Param1 { get; set; }
        public T2 Param2 { get; set; }
        public T3 Param3 { get; set; }
        public T4 Param4 { get; set; }
        public T5 Param5 { get; set; }
        public T6 Param6 { get; set; }
        public T7 Param7 { get; set; }
        public T8 Param8 { get; set; }
        public T9 Param9 { get; set; }
    }

    public class EventArgs<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> : EventArgs
    {
        public EventArgs(T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8, T9 param9, T10 param10)
        {
            Param1 = param1;
            Param2 = param2;
            Param3 = param3;
            Param4 = param4;
            Param5 = param5;
            Param6 = param6;
            Param7 = param7;
            Param8 = param8;
            Param9 = param9;
            Param10 = param10;
        }

        public T1 Param1 { get; set; }
        public T2 Param2 { get; set; }
        public T3 Param3 { get; set; }
        public T4 Param4 { get; set; }
        public T5 Param5 { get; set; }
        public T6 Param6 { get; set; }
        public T7 Param7 { get; set; }
        public T8 Param8 { get; set; }
        public T9 Param9 { get; set; }
        public T10 Param10 { get; set; }
    }

    public class EventArgs<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> : EventArgs
    {
        public EventArgs(T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8, T9 param9, T10 param10, T11 param11)
        {
            Param1 = param1;
            Param2 = param2;
            Param3 = param3;
            Param4 = param4;
            Param5 = param5;
            Param6 = param6;
            Param7 = param7;
            Param8 = param8;
            Param9 = param9;
            Param10 = param10;
            Param11 = param11;
        }

        public T1 Param1 { get; set; }
        public T2 Param2 { get; set; }
        public T3 Param3 { get; set; }
        public T4 Param4 { get; set; }
        public T5 Param5 { get; set; }
        public T6 Param6 { get; set; }
        public T7 Param7 { get; set; }
        public T8 Param8 { get; set; }
        public T9 Param9 { get; set; }
        public T10 Param10 { get; set; }
        public T11 Param11 { get; set; }
    }

    public class EventArgs<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> : EventArgs
    {
        public EventArgs(T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8, T9 param9, T10 param10, T11 param11, T12 param12)
        {
            Param1 = param1;
            Param2 = param2;
            Param3 = param3;
            Param4 = param4;
            Param5 = param5;
            Param6 = param6;
            Param7 = param7;
            Param8 = param8;
            Param9 = param9;
            Param10 = param10;
            Param11 = param11;
            Param12 = param12;
        }

        public T1 Param1 { get; set; }
        public T2 Param2 { get; set; }
        public T3 Param3 { get; set; }
        public T4 Param4 { get; set; }
        public T5 Param5 { get; set; }
        public T6 Param6 { get; set; }
        public T7 Param7 { get; set; }
        public T8 Param8 { get; set; }
        public T9 Param9 { get; set; }
        public T10 Param10 { get; set; }
        public T11 Param11 { get; set; }
        public T12 Param12 { get; set; }
    }

    public class EventArgs<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> : EventArgs
    {
        public EventArgs(T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8, T9 param9, T10 param10, T11 param11, T12 param12, T13 param13)
        {
            Param1 = param1;
            Param2 = param2;
            Param3 = param3;
            Param4 = param4;
            Param5 = param5;
            Param6 = param6;
            Param7 = param7;
            Param8 = param8;
            Param9 = param9;
            Param10 = param10;
            Param11 = param11;
            Param12 = param12;
            Param13 = param13;
        }

        public T1 Param1 { get; set; }
        public T2 Param2 { get; set; }
        public T3 Param3 { get; set; }
        public T4 Param4 { get; set; }
        public T5 Param5 { get; set; }
        public T6 Param6 { get; set; }
        public T7 Param7 { get; set; }
        public T8 Param8 { get; set; }
        public T9 Param9 { get; set; }
        public T10 Param10 { get; set; }
        public T11 Param11 { get; set; }
        public T12 Param12 { get; set; }
        public T13 Param13 { get; set; }
    }

    public class EventArgs<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> : EventArgs
    {
        public EventArgs(T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8, T9 param9, T10 param10, T11 param11, T12 param12, T13 param13, T14 param14)
        {
            Param1 = param1;
            Param2 = param2;
            Param3 = param3;
            Param4 = param4;
            Param5 = param5;
            Param6 = param6;
            Param7 = param7;
            Param8 = param8;
            Param9 = param9;
            Param10 = param10;
            Param11 = param11;
            Param12 = param12;
            Param13 = param13;
            Param14 = param14;
        }

        public T1 Param1 { get; set; }
        public T2 Param2 { get; set; }
        public T3 Param3 { get; set; }
        public T4 Param4 { get; set; }
        public T5 Param5 { get; set; }
        public T6 Param6 { get; set; }
        public T7 Param7 { get; set; }
        public T8 Param8 { get; set; }
        public T9 Param9 { get; set; }
        public T10 Param10 { get; set; }
        public T11 Param11 { get; set; }
        public T12 Param12 { get; set; }
        public T13 Param13 { get; set; }
        public T14 Param14 { get; set; }
    }

    public class EventArgs<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> : EventArgs
    {
        public EventArgs(T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8, T9 param9, T10 param10, T11 param11, T12 param12, T13 param13, T14 param14, T15 param15)
        {
            Param1 = param1;
            Param2 = param2;
            Param3 = param3;
            Param4 = param4;
            Param5 = param5;
            Param6 = param6;
            Param7 = param7;
            Param8 = param8;
            Param9 = param9;
            Param10 = param10;
            Param11 = param11;
            Param12 = param12;
            Param13 = param13;
            Param14 = param14;
            Param15 = param15;
        }

        public T1 Param1 { get; set; }
        public T2 Param2 { get; set; }
        public T3 Param3 { get; set; }
        public T4 Param4 { get; set; }
        public T5 Param5 { get; set; }
        public T6 Param6 { get; set; }
        public T7 Param7 { get; set; }
        public T8 Param8 { get; set; }
        public T9 Param9 { get; set; }
        public T10 Param10 { get; set; }
        public T11 Param11 { get; set; }
        public T12 Param12 { get; set; }
        public T13 Param13 { get; set; }
        public T14 Param14 { get; set; }
        public T15 Param15 { get; set; }
    }

    public class EventArgs<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16> : EventArgs
    {
        public EventArgs(T1 param1, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6, T7 param7, T8 param8, T9 param9, T10 param10, T11 param11, T12 param12, T13 param13, T14 param14, T15 param15, T16 param16)
        {
            Param1 = param1;
            Param2 = param2;
            Param3 = param3;
            Param4 = param4;
            Param5 = param5;
            Param6 = param6;
            Param7 = param7;
            Param8 = param8;
            Param9 = param9;
            Param10 = param10;
            Param11 = param11;
            Param12 = param12;
            Param13 = param13;
            Param14 = param14;
            Param15 = param15;
            Param16 = param16;
        }

        public T1 Param1 { get; set; }
        public T2 Param2 { get; set; }
        public T3 Param3 { get; set; }
        public T4 Param4 { get; set; }
        public T5 Param5 { get; set; }
        public T6 Param6 { get; set; }
        public T7 Param7 { get; set; }
        public T8 Param8 { get; set; }
        public T9 Param9 { get; set; }
        public T10 Param10 { get; set; }
        public T11 Param11 { get; set; }
        public T12 Param12 { get; set; }
        public T13 Param13 { get; set; }
        public T14 Param14 { get; set; }
        public T15 Param15 { get; set; }
        public T16 Param16 { get; set; }
    }
}
