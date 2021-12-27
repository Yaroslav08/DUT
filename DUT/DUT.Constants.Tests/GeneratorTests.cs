using Xunit;
namespace DUT.Constants.Tests
{
    public class GeneratorTests
    {
        [Fact]
        public void GroupInviteCodeLength()
        {
            var code = Generator.CreateGroupInviteCode();
            Assert.Equal(9, code.Length);
        }

        [Theory]
        [InlineData(1000, 0)]
        [InlineData(10000, 1)]
        [InlineData(100000, 5)]
        public void CheckOnUniqGroupInviteCode(int commonCount, int maxRepeat)
        {
            Dictionary<string, int> dictionaryOfInviteCodes = new Dictionary<string, int>();

            string code = "";

            for (int i = 0; i < commonCount; i++)
            {
                code = Generator.CreateGroupInviteCode();
                if (dictionaryOfInviteCodes.ContainsKey(code))
                {
                    dictionaryOfInviteCodes[code]++;
                }
                else
                {
                    dictionaryOfInviteCodes.Add(code, 1);
                }
            }

            var count = dictionaryOfInviteCodes.Where(x => x.Value > 1).Count();

            Assert.True(count <= maxRepeat);
        }
    }
}