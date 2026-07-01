using DertInfo.CrossCutting.Auth;
using DertInfo.Repository;
using DertInfo.Services.Entity.Dances;
using DertInfo.Services.Entity.DanceScoreParts;
using DertInfo.Services.Entity.JudgeSlots;
using NSubstitute;

namespace DertInfo.Services.UTests
{
    public class JudgeSlotService_Setup
    {
        protected JudgeSlotService sut;
        protected JudgeSlotService_Setup_Data data;

        protected IDanceScorePartsService mockDanceScorePartsService;
        protected IDanceService mockDanceService;

        protected IJudgeSlotRepository mockJudgeSlotRepository;

        protected IDertInfoUser mockDertInfoUser;


        public JudgeSlotService_Setup()
        {
            // Construct the dependencies
            this.mockDertInfoUser = Substitute.For<IDertInfoUser>();
            this.mockDanceService = Substitute.For<IDanceService>();
            this.mockDanceService = Substitute.For<IDanceService>();
            this.mockDanceScorePartsService = Substitute.For<IDanceScorePartsService>();
            this.mockJudgeSlotRepository = Substitute.For<IJudgeSlotRepository>();

            // Create the data service
            this.data = new JudgeSlotService_Setup_Data();

            // Build the sut
            this.sut = new JudgeSlotService(mockDanceService, mockDanceScorePartsService, mockJudgeSlotRepository, mockDertInfoUser);
        }
    }
}
