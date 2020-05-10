using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Domain.Testing.Models
{
    public class DomainSeed<T>
    {
        /// <summary>
        /// Having bias be a float would be much "cleaner"
        /// </summary>
        public int bias;
        public T value;

        public DomainSeed(T Value, int Bias)
        {
            value = Value;
            bias = Bias;
        }
    }
}
