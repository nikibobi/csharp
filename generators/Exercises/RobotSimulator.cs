using Generators;
using Generators.Input;
using Generators.Output;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;

namespace Generators.Exercises
{
    public class RobotSimulator : Exercise
    {
        private const string ObjectName = "robot";

        private const string PropCreate = "create";
        private const string PropInstructions = "instructions";

        private const string ParamPosition = "position";
        private const string ParamX = "x";
        private const string ParamY = "y";
        private const string ParamDirection = "direction";

        protected override void UpdateCanonicalData(CanonicalData canonicalData)
        {
            foreach (var canonicalDataCase in canonicalData.Cases)
            {
                canonicalDataCase.TestedMethodType = TestedMethodType.Instance;
                canonicalDataCase.SetConstructorInputParameters("robot");

                if (canonicalDataCase.Property == "create")
                {
                    //canonicalDataCase.Property = null;
                }
                else if (canonicalDataCase.Property == "instructions")
                {
                    canonicalDataCase.Property = "Simulate";
                    canonicalDataCase.SetInputParameters("instructions");
                }
            }
        }

        protected override string RenderTestMethodBodyArrange(TestMethodBody testMethodBody)
        {
            const string template = @"var {{ObjectName}} = new RobotSimulator(Bearing.{{Bearing}}, new Coordinate({{X}}, {{Y}}));";

            dynamic input = testMethodBody.CanonicalDataCase.ConstructorInput["robot"];

            testMethodBody.AssertTemplateParameters = new
            {
                ObjectName,
                X = input["position"]["x"],
                Y = input["position"]["y"],
                Bearing = input["direction"]
            };

            return TemplateRenderer.RenderInline(template.ToString(), testMethodBody.AssertTemplateParameters);
        }

        protected override string RenderTestMethodBodyAssert(TestMethodBody testMethodBody)
        {
            var template = new StringBuilder();
            var expected = (IDictionary<string, dynamic>)testMethodBody.CanonicalDataCase.Expected;

            if (expected.ContainsKey("position"))
            {
                template.AppendLine(@"Assert.Equal(new Coordinate({{X}}, {{Y}}), {{ObjectName}}.Coordinate);");
                testMethodBody.AssertTemplateParameters = new
                {
                    ObjectName,
                    X = expected["position"]["x"],
                    Y = expected["position"]["y"]
                };
            }

            if (expected.ContainsKey("direction"))
            {
                template.AppendLine(@"Assert.Equal(Bearing.{{Bearing}}, {{ObjectName}}.Bearing);");
                testMethodBody.AssertTemplateParameters = new
                {
                    ObjectName,
                    Bearing = expected["direction"]
                };
            }

            if (expected.ContainsKey("position") && expected.ContainsKey("direction"))
            {
                testMethodBody.AssertTemplateParameters = new
                {
                    ObjectName,
                    X = expected["position"]["x"],
                    Y = expected["position"]["y"],
                    Bearing = expected["direction"]
                };
            }

            return TemplateRenderer.RenderInline(template.ToString(), testMethodBody.AssertTemplateParameters);
        }
    }
}