using FluentAssertions;
using Moq;
using MossWPF.Services;
namespace MossWPF.Tests
{
    public class ServiceTests
    {
        
        [Fact]
        public async void ShouldParseResultsFromUrl()
        {
            string url = "<HTML><HEAD><TITLE>Moss Results</TITLE></HEAD><BODY>Moss Results<p>Sun Dec 10 07:38:34 PST 2023<p>Options -l cc -m 10<HR>[ <A HREF=\"http://moss.stanford.edu/general/format.html\" TARGET=\"_top\"> How to Read the Results</A> | <A HREF=\"http://moss.stanford.edu/general/tips.html\" TARGET=\"_top\"> Tips</A> | <A HREF=\"http://moss.stanford.edu/general/faq.html\"> FAQ</A> | <A HREF=\"mailto:moss-request@cs.stanford.edu\">Contact</A> | <A HREF=\"http://moss.stanford.edu/general/scripts.html\">Submission Scripts</A> | <A HREF=\"http://moss.stanford.edu/general/credits.html\" TARGET=\"_top\"> Credits</A> ]<HR><TABLE><TR><TH>File 1<TH>File 2<TH>Lines Matched<TR><TD><A HREF=\"http://moss.stanford.edu/results/0/123456789012/match0.html\">C:/SomeDirectory/palindrome/src/carddeck.cpp (99%)</A>    <TD><A HREF=\"http://moss.stanford.edu/results/0/123456789012/match0.html\">C:/SomeDirectory/palindrome/src/carddeck2.cpp (99%)</A><TD ALIGN=right>101<TR><TD><A HREF=\"http://moss.stanford.edu/results/0/123456789012/match1.html\">C:/SomeDirectory/palindrome/base/base.cpp (86%)</A>    <TD><A HREF=\"http://moss.stanford.edu/results/0/123456789012/match1.html\">C:/SomeDirectory/palindrome/src/carddeck2.cpp (5%)</A><TD ALIGN=right>9<TR><TD><A HREF=\"http://moss.stanford.edu/results/0/123456789012/match2.html\">C:/SomeDirectory/palindrome/base/base.cpp (86%)</A>    <TD><A HREF=\"http://moss.stanford.edu/results/0/123456789012/match2.html\">C:/SomeDirectory/palindrome/src/carddeck.cpp (5%)</A><TD ALIGN=right>9<TR><TD><A HREF=\"http://moss.stanford.edu/results/0/123456789012/match3.html\">C:/SomeDirectory/C89Demo/C89Demo/C89Demo.cpp (87%)</A>    <TD><A HREF=\"http://moss.stanford.edu/results/0/123456789012/match3.html\">C:/SomeDirectory/C89_Copy/C89_Copy/C89_Copy.cpp (87%)</A><TD ALIGN=right>3<TR><TD><A HREF=\"http://moss.stanford.edu/results/0/123456789012/match4.html\">C:/SomeDirectory/C89Demo/C89Demo/C89Demo.cpp (87%)</A>    <TD><A HREF=\"http://moss.stanford.edu/results/0/123456789012/match4.html\">C:/SomeDirectory/C89_Copy/C89_Copy/C89_Copy.cpp (87%)</A><TD ALIGN=right>3<TR><TD><A HREF=\"http://moss.stanford.edu/results/0/123456789012/match5.html\">C:/SomeDirectory/C89Demo/C89Demo/C89Demo.cpp (87%)</A>    <TD><A HREF=\"http://moss.stanford.edu/results/0/123456789012/match5.html\">C:/SomeDirectory/C89_Copy/C89_Copy/C89_Copy.cpp (87%)</A><TD ALIGN=right>3<TR><TD><A HREF=\"http://moss.stanford.edu/results/0/123456789012/match6.html\">C:/SomeDirectory/C89Demo/C89Demo/C89Demo.cpp (87%)</A>    <TD><A HREF=\"http://moss.stanford.edu/results/0/123456789012/match6.html\">C:/SomeDirectory/C89_Copy/C89_Copy/C89_Copy.cpp (87%)</A><TD ALIGN=right>3<TR><TD><A HREF=\"http://moss.stanford.edu/results/0/123456789012/match7.html\">C:/SomeDirectory/C89Demo/C89Demo/C89Demo.cpp (87%)</A>    <TD><A HREF=\"http://moss.stanford.edu/results/0/123456789012/match7.html\">C:/SomeDirectory/C89_Copy/C89_Copy/C89_Copy.cpp (87%)</A><TD ALIGN=right>3<TR><TD><A HREF=\"http://moss.stanford.edu/results/0/123456789012/match8.html\">C:/SomeDirectory/C89Demo/C89Demo/C89Demo.cpp (87%)</A>    <TD><A HREF=\"http://moss.stanford.edu/results/0/123456789012/match8.html\">C:/SomeDirectory/C89_Copy/C89_Copy/C89_Copy.cpp (87%)</A><TD ALIGN=right>3<TR><TD><A HREF=\"http://moss.stanford.edu/results/0/123456789012/match9.html\">C:/SomeDirectory/C89Demo/C89Demo/C89Demo.cpp (87%)</A>    <TD><A HREF=\"http://moss.stanford.edu/results/0/123456789012/match9.html\">C:/SomeDirectory/C89_Copy/C89_Copy/C89_Copy.cpp (87%)</A><TD ALIGN=right>3</TABLE><HR>Any errors encountered during this query are listed below.<p></BODY></HTML>";
            var resultParser = new ResultParser(new Mock<IHttpClientFactory>().Object);
            var result = await resultParser.ExtractItemsAndHrefs(url);
            result.Should().HaveCount(10);
            result.Select(r => r.LinesMatched).ToList().ForEach(x => x.Should().NotBe(0));
        }


        [Fact]
        public void ExtractFilePathAndPercentage_ValidInput_ReturnsFilePathAndPercentage()
        {
            // Arrange
            string inputString = "C:/SomeDirectory/palindrome/src/carddeck.cpp (99%)";
            string expectedFilePath = "C:/SomeDirectory/palindrome/src/carddeck.cpp";
            int expectedPercentage = 99;

            // Act
            ResultParser.ExtractFilePathAndPercentage(inputString, out var filePath, out var percentage);

            // Assert
            filePath.Should().Be(expectedFilePath);
            percentage.Should().Be(expectedPercentage);
        }

        [Fact]
        public void ExtractFilePathAndPercentage_NoPercentage_ReturnsEmptyPercentage()
        {
            // Arrange
            string inputString = "C:/SomeDirectory/palindrome/src/carddeck.cpp";
            string expectedFilePath = "";
            int expectedPercentage = 0;

            // Act
            ResultParser.ExtractFilePathAndPercentage(inputString, out var filePath, out var percentage);

            // Assert
            filePath.Should().Be(expectedFilePath);
            percentage.Should().Be(expectedPercentage);
        }

        [Fact]
        public void ExtractFilePathAndPercentage_InvalidInput_ReturnsEmptyFilePathAndPercentage()
        {
            // Arrange
            string inputString = "Invalid input";
            string expectedFilePath = string.Empty;
            int expectedPercentage = 0;

            // Act
            ResultParser.ExtractFilePathAndPercentage(inputString, out var filePath, out var percentage);

            // Assert
            filePath.Should().Be(expectedFilePath);
            percentage.Should().Be(expectedPercentage);
        }
    }

}
