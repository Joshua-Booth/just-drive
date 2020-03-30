using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class CarControllerUnitTests
    {
        private CarController car;

        [SetUp]
        public void CreateGameObject()
        {
            car = new CarController();
        }

        [Test]
        public void CheckIncreaseDifficulty_UnitTest()
        {
            var tempDifficulty = car.speedDifficulty;
            car.increaseDifficulty();

            //Asserts that the difficulty has increased
            Assert.Less(tempDifficulty, car.speedDifficulty);
        }

        [Test]
        public void CheckSetUpEngineAudioSource_UnitTest()
        {
            AudioClip audioSource = null;

            // Asserts that audio clip is null
            try
            {
                Assert.IsNull(car.SetUpEngineAudioSource(audioSource));
                Assert.Fail();
            }
            catch (System.NullReferenceException)
            {
                Assert.Pass();
            }
        }

        [Test]
        public void CheckULerp_UnitTest()
        {
            //Asserts that ULerp returned a valid value
            Assert.GreaterOrEqual(car.ULerp(5, 10, 5), 25);

            // Asserts that ULerp returned zero
            Assert.Zero(car.ULerp(0, 0, 0));

            // Asserts that ULerp returned an error
            Assert.IsNotNull(car.ULerp(123321412421244, 0, 999999999999999));
        }

        [TearDown]
        public void DestroyGameObject()
        {
            car = null;
        }
    }
}
