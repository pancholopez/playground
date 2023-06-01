namespace DockerSupport.UnitTests;

public class Tests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void CountShouldControlNumberOfResults()
    {
        IEnumerable<int> numberSequence = Enumerable.Range(1, 10);
        Assert.That(numberSequence.Count(), Is.EqualTo(10));
    }

    [Test]
    public void Index_IsNotNull()
    {
        // just to force reference web project during container build
        var index = new Web.Pages.IndexModel(null!);
        
        Assert.That(index, Is.Not.Null);
    }
}