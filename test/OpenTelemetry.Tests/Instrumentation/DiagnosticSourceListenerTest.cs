// <copyright file="DiagnosticSourceListenerTest.cs" company="OpenTelemetry Authors">
// Copyright The OpenTelemetry Authors
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </copyright>

using System.Diagnostics;
using Xunit;

namespace OpenTelemetry.Instrumentation.Tests
{
    public class DiagnosticSourceListenerTest
    {
        private const string TestSourceName = "TestSourceName";
        private DiagnosticSource diagnosticSource;
        private TestListenerHandler testListenerHandler;
        private DiagnosticSourceSubscriber testDiagnosticSourceSubscriber;

        public DiagnosticSourceListenerTest()
        {
            this.diagnosticSource = new DiagnosticListener(TestSourceName);
            this.testListenerHandler = new TestListenerHandler(TestSourceName);
            this.testDiagnosticSourceSubscriber = new DiagnosticSourceSubscriber(this.testListenerHandler, null);
            this.testDiagnosticSourceSubscriber.Subscribe();
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void ListenerHandlerIsNotInvokedWhenSuppressInstrumentationTrue(bool suppressInstrumentation)
        {
            using var scope = SuppressInstrumentationScope.Begin(suppressInstrumentation);

            var activity = new Activity("Main");
            this.diagnosticSource.StartActivity(activity, null);
            this.diagnosticSource.StopActivity(activity, null);

            if (suppressInstrumentation)
            {
                Assert.Equal(0, this.testListenerHandler.OnStartInvokedCount);
                Assert.Equal(0, this.testListenerHandler.OnStopInvokedCount);
            }
            else
            {
                Assert.Equal(1, this.testListenerHandler.OnStartInvokedCount);
                Assert.Equal(1, this.testListenerHandler.OnStopInvokedCount);
            }
        }
    }
}
