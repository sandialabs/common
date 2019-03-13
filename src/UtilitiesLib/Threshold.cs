using System;

namespace gov.sandia.sld.common.utilities
{
    /// <summary>
    /// Keep track of a transition point between two things
    /// 
    /// Mirriam-Webster definition:
    /// a level, point, or value above which something is true or will take place and below which it is not or will not
    /// </summary>
    public class Threshold
    {
        public virtual double Transition { get => _transition; protected set => _transition = value; }

        public Threshold(double transition)
        {
            Transition = transition;
        }

        protected double _transition;
    }

    /// <summary>
    /// When we want to have two transition points
    /// </summary>
    public class Threshold2 : Threshold
    {
        public virtual double Transition2 { get => _transition2; protected set => _transition2 = value; }

        public Threshold2(double transition, double transition2)
            : base(transition)
        {
            Transition2 = transition2;

            if (_transition > _transition2)
            {
                double temp = _transition;
                _transition = _transition2;
                _transition2 = temp;
            }
        }

        protected double _transition2;
    }

    /// <summary>
    /// When we want to record a threshold between two sections and restrict the range to 0 and 100
    /// 
    /// Make sure the transition point is >= 0 and <= 100
    /// </summary>
    public class PercentThreshold : Threshold
    {
        public override double Transition { get => _transition; protected set => _transition = Math.Min(Math.Max(value, 0), 100.0); }

        public PercentThreshold(double transition)
            : base(transition)
        {
        }
    }

    /// <summary>
    /// When we want to have two transition points
    /// 
    /// Make sure both transition points are >= 0 and <= 100
    /// </summary>
    public class PercentThreshold2 : Threshold2
    {
        public override double Transition { get => _transition; protected set => _transition = Math.Min(Math.Max(value, 0), 100.0); }
        public override double Transition2 { get => _transition2; protected set => _transition2 = Math.Min(Math.Max(value, 0), 100.0); }

        public PercentThreshold2(double transition, double transition2)
            : base(transition, transition2)
        {
        }
    }
}
