using DertInfo.Repository;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

namespace DertInfo.Services
{
    public interface IPricingService
    {
        Task<decimal> GetCurrentPriceForRegistration(int registrationId);
    }

    public class PricingService : IPricingService
    {
        private readonly IMemberAttendanceRepository _memberAttendanceRepository;
        private readonly ITeamAttendanceRepository _teamAttendanceRepository;

        public PricingService(
            IMemberAttendanceRepository memberAttendanceRepository,
            ITeamAttendanceRepository teamAttendanceRepository
            )
        {
            _memberAttendanceRepository = memberAttendanceRepository;
            _teamAttendanceRepository = teamAttendanceRepository;
        }

        public async Task<decimal> GetCurrentPriceForRegistration(int registrationId)
        {
            decimal membersTotalPrice = await _memberAttendanceRepository.SumAttendanceSalesForRegistration(registrationId);
            decimal teamsTotalPrice = await _teamAttendanceRepository.SumAttendanceSalesForRegistration(registrationId);

            return membersTotalPrice + teamsTotalPrice;
        }
    }
}
