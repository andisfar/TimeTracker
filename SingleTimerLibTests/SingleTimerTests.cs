using Microsoft.VisualStudio.TestTools.UnitTesting;
using SingleTimerLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SingleTimerLib.Tests
{
    [TestClass()]
    public class SingleTimerTests
    {
        [TestMethod()]
        public void OnElapsedTimeChangingTest()
        {
            TestATimerElapsedTimeChaning();
        }

        private static void TestATimerElapsedTimeChaning()
        {
            var eventHandlers = new SingleTimerEventHandlers
            {
                ElapsedTimeChanging = TimerTest_ElapsedTimeChanging,
                NameChaning = TimerTest_NameChanging,
                ResetTimer = TimerTest_TimerReset
            };
            using (SingleTimer t = new SingleTimer(0, "Test Timer", "05:05:05", eventHandlers))
            {
                Assert.IsInstanceOfType(t, typeof(SingleTimer));
                t.StartOrStop();
                for (int i = 0; i < 10; ++i)
                {
                }
            }
        }

        [TestMethod()]
        public void SingleTimerTest()
        {
            using (SingleTimer t = new SingleTimer(0, "Test Timer"))
            {
                Assert.IsInstanceOfType(t, typeof(SingleTimer));
            }
        }

        [TestMethod()]
        public void SingleTimerTest1()
        {
            var eventHandlers = new SingleTimerEventHandlers
            {
                ElapsedTimeChanging = TimerTest_ElapsedTimeChanging,
                NameChaning = TimerTest_NameChanging,
                ResetTimer = TimerTest_TimerReset
            };
       
            using (SingleTimer t = new SingleTimer(0, "Test Timer", "05:05:05", eventHandlers))            
            {
                Assert.IsInstanceOfType(t, typeof(SingleTimer));
                t.StartOrStop();
                t.ResetTimer();
                t.ReNameTimer("Timer Test ReName");
            }
        }

        private static void TimerTest_TimerReset(object sender, SingleTimerLibEventArgs e)
        {
            Assert.IsInstanceOfType(e.Timer, typeof(SingleTimer));
        }

        private static void TimerTest_NameChanging(object sender, SingleTimerNameChangingEventArgs e, [System.Runtime.CompilerServices.CallerMemberName] string caller = "")
        {
            Assert.IsInstanceOfType(e.Timer, typeof(SingleTimer));
        }

        private static void TimerTest_ElapsedTimeChanging(object sender, SingleTimerElapsedTimeChangingEventArgs e, [System.Runtime.CompilerServices.CallerMemberName] string caller = "")
        {
            Assert.IsInstanceOfType(e.Timer, typeof(SingleTimer));
            e.Timer.Log_Message(InfoTypes.Default);
        }

        [TestMethod()]
        public void StartOrStopTest()
        {
            using (SingleTimer t = new SingleTimer(0, "Test Timer"))
            {
                Assert.IsFalse(t.IsRunning);
                t.StartOrStop();
                Assert.IsTrue(t.IsRunning);
            }
        }

        [TestMethod()]
        public void HandleTimerElapsedTest()
        {
            TestATimerElapsedTimeChaning();
        }

        [TestMethod()]
        public void SetElapsedTimeTest()
        {
            var eventHandlers = new SingleTimerEventHandlers
            {
                ElapsedTimeChanging = TimerTest_ElapsedTimeChanging,
                NameChaning = TimerTest_NameChanging,
                ResetTimer = TimerTest_TimerReset
            };

            using (SingleTimer t = new SingleTimer(0, "Test Timer", "05:05:05", eventHandlers))
            {
                Assert.IsInstanceOfType(t, typeof(SingleTimer));
                t.TimerReset += TimerTest_TimerReset;
                t.NameChanging += TimerTest_NameChanging;
                t.ElapsedTimeChanging += TimerTest_ElapsedTimeChanging;
                t.StartOrStop();
                for (int i = 0; i < 10; ++i)
                {

                    Assert.IsInstanceOfType(t, typeof(SingleTimer));
                    t.StartOrStop();
                    t.Log_Message(InfoTypes.Default);
                }
            }
        }

        [TestMethod()]
        public void ResetTimerTest()
        {
            TestATimerElapsedTimeChaning();
        }

        [TestMethod()]
        public void Log_MessageTest()
        {
            using (SingleTimer t = new SingleTimer(0, "Test Timer"))
            {
                t.StartOrStop();
                Assert.IsTrue(t.IsRunning);
                t.Log_Message(InfoTypes.TimerEvents);
                t.StartOrStop();
                Assert.IsFalse(t.IsRunning);
            }
        }

        [TestMethod()]
        public void StopTimerTest()
        {
            using (SingleTimer t = new SingleTimer(0, "Test Timer"))
            {
                t.StartOrStop();
                Assert.IsTrue(t.IsRunning);
                t.StartOrStop();
                Assert.IsFalse(t.IsRunning);
            }
        }

        [TestMethod()]
        public void StartTimerTest()
        {
            using (SingleTimer t = new SingleTimer(0, "Test Timer"))
            {
                t.StartOrStop();
                Assert.IsTrue(t.IsRunning);
            }
        }

        [TestMethod()]
        public void DisposeTest()
        {
            SingleTimer t = null;
            using (t = new SingleTimer(0, "Test Timer"))
            {
                t.StartOrStop();
                Assert.IsTrue(t.IsRunning);
                t.Dispose();
                Assert.ThrowsException<ObjectDisposedException>(new Action(t.StartOrStop));
            }
        }

        [TestMethod()]
        public void ReNameTimerTest()
        {
            using (SingleTimer t = new SingleTimer(0, "Test Timer"))
            {
                Assert.AreEqual("Test Timer", t.CanonicalName);
                t.ReNameTimer("Timer Test");
                Assert.AreNotEqual("Test Timer", t.CanonicalName);
            }
        }
    }
}