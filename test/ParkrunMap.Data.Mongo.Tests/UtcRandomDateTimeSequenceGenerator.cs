using System;
using AutoFixture;
using AutoFixture.Kernel;

namespace ParkrunMap.Data.Mongo.Tests
{
    internal class UtcRandomDateTimeSequenceGenerator : ISpecimenBuilder
    {
        private readonly ISpecimenBuilder _innerRandomDateTimeSequenceGenerator;

        internal UtcRandomDateTimeSequenceGenerator()
        {
            _innerRandomDateTimeSequenceGenerator =
                new RandomDateTimeSequenceGenerator();
        }

        public object Create(object request, ISpecimenContext context)
        {
            var result =
                _innerRandomDateTimeSequenceGenerator.Create(request, context);

            if (result is NoSpecimen)
                return result;

            return ((DateTime)result).ToUniversalTime();
        }
    }
}