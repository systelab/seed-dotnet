namespace Allure.builder
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using attributes;
    using Commons;

    public class Test
    {
        public enum Fixture
        {
            BeforeFeature,
            BeforeScenario,
            AfterFeature,
            AfterScenario
        }

        private static AllureLifecycle instance;

        public static void createInstance()
        {
            instance = AllureLifecycle.Instance;
        }

        public static (string path, byte[] content) addAttachment(Attachment attach)
        {
            File.WriteAllText(attach.source, attach.name);
            return (attach.source, File.ReadAllBytes(attach.source));
        }

        public static string addTest(testDefinition test)
        {
            TestResult tr = new TestResult();
            tr.labels = new List<Label>();
            if (string.IsNullOrWhiteSpace(test.id)) test.id = Guid.NewGuid().ToString("N");
            tr.uuid = test.id;
            tr.name = test.name;
            tr.description = test.description;
            if (!string.IsNullOrWhiteSpace(test.epicName)) tr.labels.Add(Label.Epic(test.epicName));
            if (!string.IsNullOrWhiteSpace(test.featureName)) tr.labels.Add(Label.Feature(test.featureName));
            if (!string.IsNullOrWhiteSpace(test.storyName)) tr.labels.Add(Label.Story(test.storyName));
            if (tr.labels.Count > 0) tr.labels.Add(Label.Thread());
            if (test.listLinks != null)
                if (test.listLinks.Count > 0)
                {
                    tr.links = new List<Link>();
                    for (int i = 0; i < test.listLinks.Count; i++)
                        if (test.listLinks[i].isIssue)
                            tr.links.Add(Link.Issue(test.listLinks[i].name, test.listLinks[i].url));
                        else
                            tr.links.Add(Link.Tms(test.listLinks[i].name, test.listLinks[i].url));
                }

            instance = instance.StartTestCase(tr);
            return tr.uuid;
        }

        public static string addTestContainer(testContainer testContainer)
        {
            TestResultContainer trc = new TestResultContainer();

            if (string.IsNullOrWhiteSpace(testContainer.id)) testContainer.id = Guid.NewGuid().ToString("N");
            trc.uuid = testContainer.id;
            if (string.IsNullOrWhiteSpace(testContainer.name)) testContainer.name = "Container Name " + trc.uuid;
            trc.name = testContainer.name;
            if (!string.IsNullOrWhiteSpace(testContainer.description))
                testContainer.description = "Container Description " + trc.uuid;
            trc.description = testContainer.description;
            if (testContainer.listLinks != null)
                if (testContainer.listLinks.Count > 0)
                {
                    trc.links = new List<Link>();
                    for (int i = 0; i < testContainer.listLinks.Count; i++)
                        if (testContainer.listLinks[i].isIssue)
                            Link.Issue(testContainer.listLinks[i].name, testContainer.listLinks[i].url);
                        else
                            Link.Tms(testContainer.listLinks[i].name, testContainer.listLinks[i].url);
                }

            instance.StartTestContainer(trc);
            return trc.uuid;
        }

        public static string addStep(step st)
        {
            StepResult str = new StepResult();

            if (string.IsNullOrWhiteSpace(st.id)) st.id = Guid.NewGuid().ToString("N");
            string uuid = st.id;
            str.name = st.name;
            str.stage = Stage.pending;
            if (st.listParamenters != null) str.parameters = st.listParamenters;
            if (st.listAttachment != null) str.attachments = st.listAttachment;
            str.description = st.description;
            instance.StartStep(uuid, str);
            return uuid;
        }

        public static bool stopStep(Status status)
        {
            try
            {
                instance.UpdateStep(x => x.status = status);
                instance.UpdateStep(x => x.stage = Stage.finished);
                instance.StopStep();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public static bool stopTest(string testId, Status status, string message, string trace)
        {
            StatusDetails std = new StatusDetails
                {flaky = false, known = true, message = message, trace = trace, muted = false};
            instance = instance.UpdateTestCase(testId, x => x.status = status);
            instance = instance.UpdateTestCase(testId, x => x.statusDetails = std);
            instance.StopTestCase(testId)
                .WriteTestCase(testId);

            return true;
        }
    }
}