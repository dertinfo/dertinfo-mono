using DertInfo.Models.Database;
using DertInfo.Models.System.Enumerations;
using DertInfo.Repository;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DertInfo.Services.Entity.CompetitionTemplates
{
    public interface ICompetitionTemplateService
    {
        Task<List<EventSetting>> ApplySwordDancingTemplate(int eventId);

        Task<List<EventSetting>> ApplyClassicSwordDancingTemplate(int eventId);
    }

    public class CompetitionTemplateService : ICompetitionTemplateService
    {
        IActivityRepository _activityRepository;
        ICompetitionScoreCategoryRepository _competitionScoreCategoryRepository;
        ICompetitionEntryAttributeRepository _competitionEntryAttributeRepository;
        ICompetitionRepository _competitionRepository;
        ICompetitionScoreSetRepository _competitionScoreSetRepository;


        public CompetitionTemplateService(
            IActivityRepository activityRepository,
            ICompetitionScoreCategoryRepository competitionScoreCategoryRepository,
            ICompetitionEntryAttributeRepository competitionEntryAttributeRepository,
            ICompetitionRepository competitionRepository,
            ICompetitionScoreSetRepository competitionScoreSetRepository
            ) {
            _activityRepository = activityRepository;
            _competitionScoreCategoryRepository = competitionScoreCategoryRepository;
            _competitionEntryAttributeRepository = competitionEntryAttributeRepository;
            _competitionRepository = competitionRepository;
            _competitionScoreSetRepository = competitionScoreSetRepository;
        }

        public async Task<List<EventSetting>> ApplySwordDancingTemplate(int eventId)
        {
            var eventSettingsToApply = new List<EventSetting>();

            #region Competitions

            //MainCompetition
            Competition mainCompetition = new Competition();
            mainCompetition.Name = "Main Competition";
            mainCompetition.CompetitionDescription = "Main Competition";
            mainCompetition.TeamEntryFee = 0;
            mainCompetition.EventId = eventId;
            mainCompetition.JudgeRequirementPerVenue = 2;
            mainCompetition.FlowState = CompetitionFlowState.New;
            mainCompetition.AccessToken = null;
            mainCompetition.IsDeleted = false;
            mainCompetition.AllowAdHocDanceAddition = false;
            mainCompetition.ResultsAreCollated = true;
            mainCompetition = await _competitionRepository.Add(mainCompetition);

            //Spotlight Competition
            Competition spotlightCompetition = new Competition();
            spotlightCompetition.Name = "Spotlight Competition";
            spotlightCompetition.CompetitionDescription = "Spotlight Competition";
            spotlightCompetition.TeamEntryFee = 0;
            spotlightCompetition.EventId = eventId;
            spotlightCompetition.JudgeRequirementPerVenue = 4;
            spotlightCompetition.FlowState = CompetitionFlowState.New;
            spotlightCompetition.AccessToken = null;
            spotlightCompetition.IsDeleted = false;
            spotlightCompetition.AllowAdHocDanceAddition = false;
            spotlightCompetition.ResultsAreCollated = true;
            spotlightCompetition = await _competitionRepository.Add(spotlightCompetition);

            //Traditional Competition
            Competition traditionalCompetition = new Competition();
            traditionalCompetition.Name = "Traditional Competition";
            traditionalCompetition.CompetitionDescription = "Traditional Competition";
            traditionalCompetition.TeamEntryFee = 0;
            traditionalCompetition.EventId = eventId;
            traditionalCompetition.JudgeRequirementPerVenue = 4;
            traditionalCompetition.FlowState = CompetitionFlowState.New;
            traditionalCompetition.AccessToken = null;
            traditionalCompetition.IsDeleted = false;
            traditionalCompetition.AllowAdHocDanceAddition = true;
            traditionalCompetition.ResultsAreCollated = false;
            traditionalCompetition = await _competitionRepository.Add(traditionalCompetition);

            //DERTy Competition
            Competition dertyCompetition = new Competition();
            dertyCompetition.Name = "Derty Competition";
            dertyCompetition.CompetitionDescription = "Derty Competition";
            dertyCompetition.TeamEntryFee = 0;
            dertyCompetition.EventId = eventId;
            dertyCompetition.JudgeRequirementPerVenue = 4;
            dertyCompetition.FlowState = CompetitionFlowState.New;
            dertyCompetition.AccessToken = null;
            dertyCompetition.IsDeleted = false;
            dertyCompetition.AllowAdHocDanceAddition = false;
            dertyCompetition.ResultsAreCollated = true;
            dertyCompetition = await _competitionRepository.Add(dertyCompetition);

            //Veterans Competition
            Competition veteransCompetition = new Competition();
            veteransCompetition.Name = "Veterans Competition";
            veteransCompetition.CompetitionDescription = "Veterans Competition";
            veteransCompetition.TeamEntryFee = 0;
            veteransCompetition.EventId = eventId;
            veteransCompetition.JudgeRequirementPerVenue = 2;
            veteransCompetition.FlowState = CompetitionFlowState.New;
            veteransCompetition.AccessToken = null;
            veteransCompetition.IsDeleted = false;
            veteransCompetition.AllowAdHocDanceAddition = false;
            veteransCompetition.ResultsAreCollated = true;
            veteransCompetition = await _competitionRepository.Add(veteransCompetition);

            #endregion

            #region Bind Competitions As Activities

            Activity activity1 = new Activity();
            activity1.EventId = eventId;
            activity1.CompetitionId = mainCompetition.Id;
            activity1.Title = mainCompetition.Name;
            activity1.Price = 0;
            activity1.IsDefault = true;
            activity1.AudienceTypeId = (int)ActivityAudienceType.TEAM;
            activity1 = await _activityRepository.Add(activity1);

            Activity activity2 = new Activity();
            activity2.EventId = eventId;
            activity2.CompetitionId = spotlightCompetition.Id;
            activity2.Title = spotlightCompetition.Name;
            activity2.Price = 0;
            activity2.IsDefault = false;
            activity2.AudienceTypeId = (int)ActivityAudienceType.TEAM;
            activity2 = await _activityRepository.Add(activity2);

            Activity activity3 = new Activity();
            activity3.EventId = eventId;
            activity3.CompetitionId = traditionalCompetition.Id;
            activity3.Title = traditionalCompetition.Name;
            activity3.Price = 0;
            activity3.IsDefault = false;
            activity3.AudienceTypeId = (int)ActivityAudienceType.TEAM;
            activity3 = await _activityRepository.Add(activity3);

            Activity activity4 = new Activity();
            activity4.EventId = eventId;
            activity4.CompetitionId = dertyCompetition.Id;
            activity4.Title = dertyCompetition.Name;
            activity4.Price = 0;
            activity4.IsDefault = false;
            activity4.AudienceTypeId = (int)ActivityAudienceType.TEAM;
            activity4 = await _activityRepository.Add(activity4);

            Activity activity5 = new Activity();
            activity5.EventId = eventId;
            activity5.CompetitionId = veteransCompetition.Id;
            activity5.Title = veteransCompetition.Name;
            activity5.Price = 0;
            activity5.IsDefault = false;
            activity5.AudienceTypeId = (int)ActivityAudienceType.TEAM;
            activity5 = await _activityRepository.Add(activity5);

            #endregion 

            #region Competition Entry Attributes

            //CompetitionEntryAttribute ceaVeterans = new CompetitionEntryAttribute();
            //ceaVeterans.Name = "Veterans";
            //ceaVeterans.Tag = "[V]";
            //ceaVeterans.CompetitionAppliesToId = mainCompetition.Id;
            //ceaVeterans.AccessToken = null;
            //ceaVeterans = await _competitionEntryAttributeRepository.Add(ceaVeterans);

            CompetitionEntryAttribute ceaSduAlt = new CompetitionEntryAttribute();
            ceaSduAlt.Name = "SDU Alternative";
            ceaSduAlt.Tag = "[A]";
            ceaSduAlt.CompetitionAppliesToId = mainCompetition.Id;
            ceaSduAlt.AccessToken = null;
            ceaSduAlt = await _competitionEntryAttributeRepository.Add(ceaSduAlt);

            CompetitionEntryAttribute ceaBestNewcomer = new CompetitionEntryAttribute();
            ceaBestNewcomer.Name = "Best Newcomer";
            ceaBestNewcomer.Tag = "[N]";
            ceaBestNewcomer.CompetitionAppliesToId = mainCompetition.Id;
            ceaBestNewcomer.AccessToken = null;
            ceaBestNewcomer = await _competitionEntryAttributeRepository.Add(ceaBestNewcomer);

            CompetitionEntryAttribute dertyYouth = new CompetitionEntryAttribute();
            dertyYouth.Name = "DERTy Youth Class";
            dertyYouth.Tag = "[Y]";
            dertyYouth.CompetitionAppliesToId = dertyCompetition.Id;
            dertyYouth.AccessToken = null;
            dertyYouth = await _competitionEntryAttributeRepository.Add(dertyYouth);

            CompetitionEntryAttribute dertyJr = new CompetitionEntryAttribute();
            dertyJr.Name = "DERTy Junior  Class";
            dertyJr.Tag = "[J]";
            dertyJr.CompetitionAppliesToId = dertyCompetition.Id;
            dertyJr.AccessToken = null;
            dertyJr = await _competitionEntryAttributeRepository.Add(dertyJr);

            CompetitionEntryAttribute premier = new CompetitionEntryAttribute();
            premier.Name = "Premier  Class";
            premier.Tag = "[P]";
            premier.CompetitionAppliesToId = mainCompetition.Id;
            premier.AccessToken = null;
            premier = await _competitionEntryAttributeRepository.Add(premier);

            CompetitionEntryAttribute champtionship = new CompetitionEntryAttribute();
            champtionship.Name = "Championship  Class";
            champtionship.Tag = "[C]";
            champtionship.CompetitionAppliesToId = mainCompetition.Id;
            champtionship.AccessToken = null;
            champtionship = await _competitionEntryAttributeRepository.Add(champtionship);

            CompetitionEntryAttribute open = new CompetitionEntryAttribute();
            open.Name = "Open  Class";
            open.Tag = "[O]";
            open.CompetitionAppliesToId = mainCompetition.Id;
            open.AccessToken = null;
            open = await _competitionEntryAttributeRepository.Add(open);

            #endregion 

            #region Score Sets

            ScoreSet ssMainA = new ScoreSet();
            ssMainA.Name = "Main Competition-SetA";
            ssMainA.Description = "Main Competition-SetA";
            ssMainA.CompetitionId = mainCompetition.Id;
            ssMainA.AccessToken = null;
            ssMainA.IsDeleted = false;
            ssMainA = await _competitionScoreSetRepository.Add(ssMainA);

            ScoreSet ssMainB = new ScoreSet();
            ssMainB.Name = "Main Competition-SetB";
            ssMainB.Description = "Main Competition-SetB";
            ssMainB.CompetitionId = mainCompetition.Id;
            ssMainB.AccessToken = null;
            ssMainB.IsDeleted = false;
            ssMainB = await _competitionScoreSetRepository.Add(ssMainB);

            ScoreSet ssSpotlightA = new ScoreSet();
            ssSpotlightA.Name = "Spotlight Competition-SetA";
            ssSpotlightA.Description = "Spotlight Competition-SetA";
            ssSpotlightA.CompetitionId = spotlightCompetition.Id;
            ssSpotlightA.AccessToken = null;
            ssSpotlightA.IsDeleted = false;
            ssSpotlightA = await _competitionScoreSetRepository.Add(ssSpotlightA);

            ScoreSet ssSpotlightB = new ScoreSet();
            ssSpotlightB.Name = "Spotlight Competition-SetB";
            ssSpotlightB.Description = "Spotlight Competition-SetB";
            ssSpotlightB.CompetitionId = spotlightCompetition.Id;
            ssSpotlightB.AccessToken = null;
            ssSpotlightB.IsDeleted = false;
            ssSpotlightB = await _competitionScoreSetRepository.Add(ssSpotlightB);

            ScoreSet ssTraditionalA = new ScoreSet();
            ssTraditionalA.Name = "Traditional Competition-SetA";
            ssTraditionalA.Description = "Traditional Competition-SetA";
            ssTraditionalA.CompetitionId = traditionalCompetition.Id;
            ssTraditionalA.AccessToken = null;
            ssTraditionalA.IsDeleted = false;
            ssTraditionalA = await _competitionScoreSetRepository.Add(ssTraditionalA);

            ScoreSet ssTraditionalB = new ScoreSet();
            ssTraditionalB.Name = "Traditional Competition-SetB";
            ssTraditionalB.Description = "Traditional Competition-SetB";
            ssTraditionalB.CompetitionId = traditionalCompetition.Id;
            ssTraditionalB.AccessToken = null;
            ssTraditionalB.IsDeleted = false;
            ssTraditionalB = await _competitionScoreSetRepository.Add(ssTraditionalB);

            ScoreSet ssTraditionalC1 = new ScoreSet();
            ssTraditionalC1.Name = "Traditional Competition-SetC1";
            ssTraditionalC1.Description = "Traditional Competition-SetC1";
            ssTraditionalC1.CompetitionId = traditionalCompetition.Id;
            ssTraditionalC1.AccessToken = null;
            ssTraditionalC1.IsDeleted = false;
            ssTraditionalC1 = await _competitionScoreSetRepository.Add(ssTraditionalC1);

            //C2 - required to match up with 4 judges
            ScoreSet ssTraditionalC2 = new ScoreSet();
            ssTraditionalC2.Name = "Traditional Competition-SetC2";
            ssTraditionalC2.Description = "Traditional Competition-SetC2";
            ssTraditionalC2.CompetitionId = traditionalCompetition.Id;
            ssTraditionalC2.AccessToken = null;
            ssTraditionalC2.IsDeleted = false;
            ssTraditionalC2 = await _competitionScoreSetRepository.Add(ssTraditionalC2);

            ScoreSet ssDertyFull = new ScoreSet();
            ssDertyFull.Name = "DERTy-FullSet";
            ssDertyFull.Description = "DERTy-FullSet";
            ssDertyFull.CompetitionId = dertyCompetition.Id;
            ssDertyFull.AccessToken = null;
            ssDertyFull.IsDeleted = false;
            ssDertyFull = await _competitionScoreSetRepository.Add(ssDertyFull);

            ScoreSet ssVeteransA = new ScoreSet();
            ssVeteransA.Name = "Veterans Competition-SetA";
            ssVeteransA.Description = "Veterans Competition-SetA";
            ssVeteransA.CompetitionId = veteransCompetition.Id;
            ssVeteransA.AccessToken = null;
            ssVeteransA.IsDeleted = false;
            ssVeteransA = await _competitionScoreSetRepository.Add(ssVeteransA);

            ScoreSet ssVeteransB = new ScoreSet();
            ssVeteransB.Name = "Veterans Competition-SetB";
            ssVeteransB.Description = "Veterans Competition-SetB";
            ssVeteransB.CompetitionId = veteransCompetition.Id;
            ssVeteransB.AccessToken = null;
            ssVeteransB.IsDeleted = false;
            ssVeteransB = await _competitionScoreSetRepository.Add(ssVeteransB);

            #endregion

            #region Score Categories

            //Main Competition

            #region Main Competition

            ScoreCategory scMainMusic = new ScoreCategory();
            scMainMusic.CompetitionAppliesToId = mainCompetition.Id;
            scMainMusic.Name = "Music [M]";
            scMainMusic.Description = "[Main] Music";
            scMainMusic.Tag = "MU";
            scMainMusic.MaxMarks = 15;
            scMainMusic.SortOrder = 10;
            scMainMusic.AccessToken = null;
            scMainMusic = await _competitionScoreCategoryRepository.Add(scMainMusic);

            ScoreCategory scMainStepping = new ScoreCategory();
            scMainStepping.CompetitionAppliesToId = mainCompetition.Id;
            scMainStepping.Name = "Stepping [M]";
            scMainStepping.Description = "[Main] Stepping";
            scMainStepping.Tag = "ST";
            scMainStepping.MaxMarks = 15;
            scMainStepping.SortOrder = 20;
            scMainStepping.AccessToken = null;
            scMainStepping = await _competitionScoreCategoryRepository.Add(scMainStepping);

            ScoreCategory scMainSwordHandling = new ScoreCategory();
            scMainSwordHandling.CompetitionAppliesToId = mainCompetition.Id;
            scMainSwordHandling.Name = "Sword Handling [M]";
            scMainSwordHandling.Description = "[Main] Sword Handling";
            scMainSwordHandling.Tag = "SH";
            scMainSwordHandling.MaxMarks = 15;
            scMainSwordHandling.SortOrder = 30;
            scMainSwordHandling.AccessToken = null;
            scMainSwordHandling = await _competitionScoreCategoryRepository.Add(scMainSwordHandling);

            ScoreCategory scMainDanceTechnique = new ScoreCategory();
            scMainDanceTechnique.CompetitionAppliesToId = mainCompetition.Id;
            scMainDanceTechnique.Name = "Dance Technique [M] ";
            scMainDanceTechnique.Description = "[Main] Dance Technique";
            scMainDanceTechnique.Tag = "DT";
            scMainDanceTechnique.MaxMarks = 15;
            scMainDanceTechnique.SortOrder = 40;
            scMainDanceTechnique.AccessToken = null;
            scMainDanceTechnique = await _competitionScoreCategoryRepository.Add(scMainDanceTechnique);

            ScoreCategory scMainPresentation = new ScoreCategory();
            scMainPresentation.CompetitionAppliesToId = mainCompetition.Id;
            scMainPresentation.Name = "Presentation [M]";
            scMainPresentation.Description = "[Main] Presentation";
            scMainPresentation.Tag = "PR";
            scMainPresentation.MaxMarks = 15;
            scMainPresentation.SortOrder = 410;
            scMainPresentation.AccessToken = null;
            scMainPresentation = await _competitionScoreCategoryRepository.Add(scMainPresentation);

            ScoreCategory scMainBuzzFactor = new ScoreCategory();
            scMainBuzzFactor.CompetitionAppliesToId = mainCompetition.Id;
            scMainBuzzFactor.Name = "Buzz Factor [M]";
            scMainBuzzFactor.Description = "[Main] Buzz Factor";
            scMainBuzzFactor.Tag = "BZ";
            scMainBuzzFactor.MaxMarks = 15;
            scMainBuzzFactor.SortOrder = 420;
            scMainBuzzFactor.AccessToken = null;
            scMainBuzzFactor = await _competitionScoreCategoryRepository.Add(scMainBuzzFactor);

            ScoreCategory scMainCharacters = new ScoreCategory();
            scMainCharacters.CompetitionAppliesToId = mainCompetition.Id;
            scMainCharacters.Name = "Characters [M]";
            scMainCharacters.Description = "[Main] Characters";
            scMainCharacters.Tag = "CH";
            scMainCharacters.MaxMarks = 10;
            scMainCharacters.SortOrder = 900;
            scMainCharacters.AccessToken = null;
            scMainCharacters = await _competitionScoreCategoryRepository.Add(scMainCharacters);

            #endregion

            #region Spotlight Competition

            //Spotlight
            ScoreCategory scSpotlightMusic = new ScoreCategory();
            scSpotlightMusic.CompetitionAppliesToId = spotlightCompetition.Id;
            scSpotlightMusic.Name = "Music [S]";
            scSpotlightMusic.Description = "[Spotlight] Music";
            scSpotlightMusic.Tag = "MU";
            scSpotlightMusic.MaxMarks = 15;
            scSpotlightMusic.SortOrder = 10;
            scSpotlightMusic.AccessToken = null;
            scSpotlightMusic = await _competitionScoreCategoryRepository.Add(scSpotlightMusic);

            ScoreCategory scSpotlightStepping = new ScoreCategory();
            scSpotlightStepping.CompetitionAppliesToId = spotlightCompetition.Id;
            scSpotlightStepping.Name = "Stepping [S]";
            scSpotlightStepping.Description = "[Spotlight] Stepping";
            scSpotlightStepping.Tag = "ST";
            scSpotlightStepping.MaxMarks = 15;
            scSpotlightStepping.SortOrder = 20;
            scSpotlightStepping.AccessToken = null;
            scSpotlightStepping = await _competitionScoreCategoryRepository.Add(scSpotlightStepping);

            ScoreCategory scSpotlightSwordHandling = new ScoreCategory();
            scSpotlightSwordHandling.CompetitionAppliesToId = spotlightCompetition.Id;
            scSpotlightSwordHandling.Name = "Sword Handling [S]";
            scSpotlightSwordHandling.Description = "[Spotlight] Sword Handling";
            scSpotlightSwordHandling.Tag = "SH";
            scSpotlightSwordHandling.MaxMarks = 15;
            scSpotlightSwordHandling.SortOrder = 30;
            scSpotlightSwordHandling.AccessToken = null;
            scSpotlightSwordHandling = await _competitionScoreCategoryRepository.Add(scSpotlightSwordHandling);

            ScoreCategory scSpotlightDanceTechnique = new ScoreCategory();
            scSpotlightDanceTechnique.CompetitionAppliesToId = spotlightCompetition.Id;
            scSpotlightDanceTechnique.Name = "Dance Technique [S] ";
            scSpotlightDanceTechnique.Description = "[Spotlight] Dance Technique";
            scSpotlightDanceTechnique.Tag = "DT";
            scSpotlightDanceTechnique.MaxMarks = 15;
            scSpotlightDanceTechnique.SortOrder = 40;
            scSpotlightDanceTechnique.AccessToken = null;
            scSpotlightDanceTechnique = await _competitionScoreCategoryRepository.Add(scSpotlightDanceTechnique);

            ScoreCategory scSpotlightPresentation = new ScoreCategory();
            scSpotlightPresentation.CompetitionAppliesToId = spotlightCompetition.Id;
            scSpotlightPresentation.Name = "Presentation [S]";
            scSpotlightPresentation.Description = "[Spotlight] Presentation";
            scSpotlightPresentation.Tag = "PR";
            scSpotlightPresentation.MaxMarks = 15;
            scSpotlightPresentation.SortOrder = 410;
            scSpotlightPresentation.AccessToken = null;
            scSpotlightPresentation = await _competitionScoreCategoryRepository.Add(scSpotlightPresentation);

            ScoreCategory scSpotlightBuzzFactor = new ScoreCategory();
            scSpotlightBuzzFactor.CompetitionAppliesToId = spotlightCompetition.Id;
            scSpotlightBuzzFactor.Name = "Buzz Factor [S]";
            scSpotlightBuzzFactor.Description = "[Spotlight] Buzz Factor";
            scSpotlightBuzzFactor.Tag = "BZ";
            scSpotlightBuzzFactor.MaxMarks = 15;
            scSpotlightBuzzFactor.SortOrder = 420;
            scSpotlightBuzzFactor.AccessToken = null;
            scSpotlightBuzzFactor = await _competitionScoreCategoryRepository.Add(scSpotlightBuzzFactor);

            ScoreCategory scSpotlightCharacters = new ScoreCategory();
            scSpotlightCharacters.CompetitionAppliesToId = spotlightCompetition.Id;
            scSpotlightCharacters.Name = "Characters [S]";
            scSpotlightCharacters.Description = "[Spotlight] Characters";
            scSpotlightCharacters.Tag = "CH";
            scSpotlightCharacters.MaxMarks = 10;
            scSpotlightCharacters.SortOrder = 900;
            scSpotlightCharacters.AccessToken = null;
            scSpotlightCharacters = await _competitionScoreCategoryRepository.Add(scSpotlightCharacters);

            #endregion 

            #region Traditional Competition

            ////Traditional - Pre 2016
            //ScoreCategory scTraditionalBuzzFactor = new ScoreCategory();
            //scTraditionalBuzzFactor.CompetitionAppliesToId = traditionalCompetition.Id;
            //scTraditionalBuzzFactor.Name = "Buzz Factor [T]";
            //scTraditionalBuzzFactor.Description = "[Traditional] Buzz Factor";
            //scTraditionalBuzzFactor.Tag = "BZ";
            //scTraditionalBuzzFactor.MaxMarks = 40;
            //scTraditionalBuzzFactor.SortOrder = 150;
            //scTraditionalBuzzFactor.AccessToken = null;
            //scoreCategoryRepo.InsertOrUpdate(scTraditionalBuzzFactor);

            //ScoreCategory scTraditionalStepping = new ScoreCategory();
            //scTraditionalStepping.CompetitionAppliesToId = traditionalCompetition.Id;
            //scTraditionalStepping.Name = "Stepping [T]";
            //scTraditionalStepping.Description = "[Traditional] Stepping";
            //scTraditionalStepping.Tag = "ST";
            //scTraditionalStepping.MaxMarks = 20;
            //scTraditionalStepping.SortOrder = 100;
            //scTraditionalStepping.AccessToken = null;
            //scoreCategoryRepo.InsertOrUpdate(scTraditionalStepping);

            //ScoreCategory scTraditionalStyle = new ScoreCategory();
            //scTraditionalStyle.CompetitionAppliesToId = traditionalCompetition.Id;
            //scTraditionalStyle.Name = "Style [T]";
            //scTraditionalStyle.Description = "[Traditional] Style";
            //scTraditionalStyle.Tag = "STL";
            //scTraditionalStyle.MaxMarks = 40;
            //scTraditionalStyle.SortOrder = 50;
            //scTraditionalStyle.AccessToken = null;
            //scoreCategoryRepo.InsertOrUpdate(scTraditionalStyle);


            //POST 2016
            ScoreCategory scTraditionalMusic = new ScoreCategory();
            scTraditionalMusic.CompetitionAppliesToId = traditionalCompetition.Id;
            scTraditionalMusic.Name = "Music [Tr]";
            scTraditionalMusic.Description = "[Trad] Music";
            scTraditionalMusic.Tag = "MU";
            scTraditionalMusic.MaxMarks = 15;
            scTraditionalMusic.SortOrder = 10;
            scTraditionalMusic.AccessToken = null;
            scTraditionalMusic = await _competitionScoreCategoryRepository.Add(scTraditionalMusic);

            ScoreCategory scTraditionalStepping = new ScoreCategory();
            scTraditionalStepping.CompetitionAppliesToId = traditionalCompetition.Id;
            scTraditionalStepping.Name = "Stepping [Tr]";
            scTraditionalStepping.Description = "[Trad] Stepping";
            scTraditionalStepping.Tag = "ST";
            scTraditionalStepping.MaxMarks = 15;
            scTraditionalStepping.SortOrder = 20;
            scTraditionalStepping.AccessToken = null;
            scTraditionalStepping = await _competitionScoreCategoryRepository.Add(scTraditionalStepping);

            ScoreCategory scTraditionalSwordHandling = new ScoreCategory();
            scTraditionalSwordHandling.CompetitionAppliesToId = traditionalCompetition.Id;
            scTraditionalSwordHandling.Name = "Sword Handling [Tr]";
            scTraditionalSwordHandling.Description = "[Trad] Sword Handling";
            scTraditionalSwordHandling.Tag = "SH";
            scTraditionalSwordHandling.MaxMarks = 15;
            scTraditionalSwordHandling.SortOrder = 30;
            scTraditionalSwordHandling.AccessToken = null;
            scTraditionalSwordHandling = await _competitionScoreCategoryRepository.Add(scTraditionalSwordHandling);

            ScoreCategory scTraditionalDanceTechnique = new ScoreCategory();
            scTraditionalDanceTechnique.CompetitionAppliesToId = traditionalCompetition.Id;
            scTraditionalDanceTechnique.Name = "Dance Technique [Tr] ";
            scTraditionalDanceTechnique.Description = "[Trad] Dance Technique";
            scTraditionalDanceTechnique.Tag = "DT";
            scTraditionalDanceTechnique.MaxMarks = 15;
            scTraditionalDanceTechnique.SortOrder = 40;
            scTraditionalDanceTechnique.AccessToken = null;
            scTraditionalDanceTechnique = await _competitionScoreCategoryRepository.Add(scTraditionalDanceTechnique);

            ScoreCategory scTraditionalPresentation = new ScoreCategory();
            scTraditionalPresentation.CompetitionAppliesToId = traditionalCompetition.Id;
            scTraditionalPresentation.Name = "Presentation [Tr]";
            scTraditionalPresentation.Description = "[Trad] Presentation";
            scTraditionalPresentation.Tag = "PR";
            scTraditionalPresentation.MaxMarks = 15;
            scTraditionalPresentation.SortOrder = 410;
            scTraditionalPresentation.AccessToken = null;
            scTraditionalPresentation = await _competitionScoreCategoryRepository.Add(scTraditionalPresentation);

            ScoreCategory scTraditionalBuzzFactor = new ScoreCategory();
            scTraditionalBuzzFactor.CompetitionAppliesToId = traditionalCompetition.Id;
            scTraditionalBuzzFactor.Name = "Buzz Factor [Tr]";
            scTraditionalBuzzFactor.Description = "[Trad] Buzz Factor";
            scTraditionalBuzzFactor.Tag = "BZ";
            scTraditionalBuzzFactor.MaxMarks = 15;
            scTraditionalBuzzFactor.SortOrder = 420;
            scTraditionalBuzzFactor.AccessToken = null;
            scTraditionalBuzzFactor = await _competitionScoreCategoryRepository.Add(scTraditionalBuzzFactor);

            ScoreCategory scTraditionalCharacters = new ScoreCategory();
            scTraditionalCharacters.CompetitionAppliesToId = traditionalCompetition.Id;
            scTraditionalCharacters.Name = "Characters [Tr]";
            scTraditionalCharacters.Description = "[Trad] Characters";
            scTraditionalCharacters.Tag = "CH";
            scTraditionalCharacters.MaxMarks = 10;
            scTraditionalCharacters.SortOrder = 900;
            scTraditionalCharacters.AccessToken = null;
            scTraditionalCharacters = await _competitionScoreCategoryRepository.Add(scTraditionalCharacters);

            ScoreCategory scTraditionalAlignment = new ScoreCategory();
            scTraditionalAlignment.CompetitionAppliesToId = traditionalCompetition.Id;
            scTraditionalAlignment.Name = "Alignment [Tr]";
            scTraditionalAlignment.Description = "[Trad] Alignment";
            scTraditionalAlignment.Tag = "AL";
            scTraditionalAlignment.MaxMarks = 100;
            scTraditionalAlignment.SortOrder = 1200;
            scTraditionalAlignment.AccessToken = null;
            scTraditionalAlignment = await _competitionScoreCategoryRepository.Add(scTraditionalAlignment);

            #endregion 

            #region DERTy

            ScoreCategory scDertyStepping = new ScoreCategory();
            scDertyStepping.CompetitionAppliesToId = dertyCompetition.Id;
            scDertyStepping.Name = "Stepping [Dy]";
            scDertyStepping.Description = "[Derty] Stepping";
            scDertyStepping.Tag = "ST";
            scDertyStepping.MaxMarks = 20;
            scDertyStepping.SortOrder = 40;
            scDertyStepping.AccessToken = null;
            scDertyStepping = await _competitionScoreCategoryRepository.Add(scDertyStepping);

            ScoreCategory scDertyFigures = new ScoreCategory();
            scDertyFigures.CompetitionAppliesToId = dertyCompetition.Id;
            scDertyFigures.Name = "Figures [Dy]";
            scDertyFigures.Description = "[Derty] Figures";
            scDertyFigures.Tag = "FIG";
            scDertyFigures.MaxMarks = 20;
            scDertyFigures.SortOrder = 30;
            scDertyFigures.AccessToken = null;
            scDertyFigures = await _competitionScoreCategoryRepository.Add(scDertyFigures);

            ScoreCategory scDertyStyle = new ScoreCategory();
            scDertyStyle.CompetitionAppliesToId = dertyCompetition.Id;
            scDertyStyle.Name = "Style [Dy]";
            scDertyStyle.Description = "[Derty] Style";
            scDertyStyle.Tag = "STL";
            scDertyStyle.MaxMarks = 20;
            scDertyStyle.SortOrder = 20;
            scDertyStyle.AccessToken = null;
            scDertyStyle = await _competitionScoreCategoryRepository.Add(scDertyStyle);

            ScoreCategory scDertySwordHandling = new ScoreCategory();
            scDertySwordHandling.CompetitionAppliesToId = dertyCompetition.Id;
            scDertySwordHandling.Name = "Sword Handling [Dy]";
            scDertySwordHandling.Description = "[Derty] Sword Handling";
            scDertySwordHandling.Tag = "SH";
            scDertySwordHandling.MaxMarks = 20;
            scDertySwordHandling.SortOrder = 10;
            scDertySwordHandling.AccessToken = null;
            scDertySwordHandling = await _competitionScoreCategoryRepository.Add(scDertySwordHandling);

            ScoreCategory scDertyTeamwork = new ScoreCategory();
            scDertyTeamwork.CompetitionAppliesToId = dertyCompetition.Id;
            scDertyTeamwork.Name = "Teamwork [Dy]";
            scDertyTeamwork.Description = "[Derty] Teamwork";
            scDertyTeamwork.Tag = "TW";
            scDertyTeamwork.MaxMarks = 20;
            scDertyTeamwork.SortOrder = 50;
            scDertyTeamwork.AccessToken = null;
            scDertyTeamwork = await _competitionScoreCategoryRepository.Add(scDertyTeamwork);

            #endregion 

            #region Veterans Competition

            //Veterans
            ScoreCategory scVeteransMusic = new ScoreCategory();
            scVeteransMusic.CompetitionAppliesToId = veteransCompetition.Id;
            scVeteransMusic.Name = "Music [V]";
            scVeteransMusic.Description = "[Veterans] Music";
            scVeteransMusic.Tag = "MU";
            scVeteransMusic.MaxMarks = 15;
            scVeteransMusic.SortOrder = 10;
            scVeteransMusic.AccessToken = null;
            scVeteransMusic = await _competitionScoreCategoryRepository.Add(scVeteransMusic);

            ScoreCategory scVeteransStepping = new ScoreCategory();
            scVeteransStepping.CompetitionAppliesToId = veteransCompetition.Id;
            scVeteransStepping.Name = "Stepping [V]";
            scVeteransStepping.Description = "[Veterans] Stepping";
            scVeteransStepping.Tag = "ST";
            scVeteransStepping.MaxMarks = 15;
            scVeteransStepping.SortOrder = 20;
            scVeteransStepping.AccessToken = null;
            scVeteransStepping = await _competitionScoreCategoryRepository.Add(scVeteransStepping);

            ScoreCategory scVeteransSwordHandling = new ScoreCategory();
            scVeteransSwordHandling.CompetitionAppliesToId = veteransCompetition.Id;
            scVeteransSwordHandling.Name = "Sword Handling [V]";
            scVeteransSwordHandling.Description = "[Veterans] Sword Handling";
            scVeteransSwordHandling.Tag = "SH";
            scVeteransSwordHandling.MaxMarks = 15;
            scVeteransSwordHandling.SortOrder = 30;
            scVeteransSwordHandling.AccessToken = null;
            scVeteransSwordHandling = await _competitionScoreCategoryRepository.Add(scVeteransSwordHandling);

            ScoreCategory scVeteransDanceTechnique = new ScoreCategory();
            scVeteransDanceTechnique.CompetitionAppliesToId = veteransCompetition.Id;
            scVeteransDanceTechnique.Name = "Dance Technique [V] ";
            scVeteransDanceTechnique.Description = "[Veterans] Dance Technique";
            scVeteransDanceTechnique.Tag = "DT";
            scVeteransDanceTechnique.MaxMarks = 15;
            scVeteransDanceTechnique.SortOrder = 40;
            scVeteransDanceTechnique.AccessToken = null;
            scVeteransDanceTechnique = await _competitionScoreCategoryRepository.Add(scVeteransDanceTechnique);

            ScoreCategory scVeteransPresentation = new ScoreCategory();
            scVeteransPresentation.CompetitionAppliesToId = veteransCompetition.Id;
            scVeteransPresentation.Name = "Presentation [V]";
            scVeteransPresentation.Description = "[Veterans] Presentation";
            scVeteransPresentation.Tag = "PR";
            scVeteransPresentation.MaxMarks = 15;
            scVeteransPresentation.SortOrder = 410;
            scVeteransPresentation.AccessToken = null;
            scVeteransPresentation = await _competitionScoreCategoryRepository.Add(scVeteransPresentation);

            ScoreCategory scVeteransBuzzFactor = new ScoreCategory();
            scVeteransBuzzFactor.CompetitionAppliesToId = veteransCompetition.Id;
            scVeteransBuzzFactor.Name = "Buzz Factor [V]";
            scVeteransBuzzFactor.Description = "[Veterans] Buzz Factor";
            scVeteransBuzzFactor.Tag = "BZ";
            scVeteransBuzzFactor.MaxMarks = 15;
            scVeteransBuzzFactor.SortOrder = 420;
            scVeteransBuzzFactor.AccessToken = null;
            scVeteransBuzzFactor = await _competitionScoreCategoryRepository.Add(scVeteransBuzzFactor);

            ScoreCategory scVeteransCharacters = new ScoreCategory();
            scVeteransCharacters.CompetitionAppliesToId = veteransCompetition.Id;
            scVeteransCharacters.Name = "Characters [V]";
            scVeteransCharacters.Description = "[Veterans] Characters";
            scVeteransCharacters.Tag = "CH";
            scVeteransCharacters.MaxMarks = 10;
            scVeteransCharacters.SortOrder = 900;
            scVeteransCharacters.AccessToken = null;
            scVeteransCharacters = await _competitionScoreCategoryRepository.Add(scVeteransCharacters);

            #endregion 

            #endregion

            #region Apply Categories To Sets

            //Main
            await _competitionScoreSetRepository.AttachScoreCategory(ssMainA, scMainMusic);
            await _competitionScoreSetRepository.AttachScoreCategory(ssMainA, scMainStepping);
            await _competitionScoreSetRepository.AttachScoreCategory(ssMainA, scMainPresentation);
            await _competitionScoreSetRepository.AttachScoreCategory(ssMainA, scMainBuzzFactor);
            await _competitionScoreSetRepository.AttachScoreCategory(ssMainA, scMainCharacters);

            await _competitionScoreSetRepository.AttachScoreCategory(ssMainB, scMainDanceTechnique);
            await _competitionScoreSetRepository.AttachScoreCategory(ssMainB, scMainSwordHandling);
            await _competitionScoreSetRepository.AttachScoreCategory(ssMainB, scMainPresentation);
            await _competitionScoreSetRepository.AttachScoreCategory(ssMainB, scMainBuzzFactor);
            await _competitionScoreSetRepository.AttachScoreCategory(ssMainB, scMainCharacters);

            //Spotlight
            await _competitionScoreSetRepository.AttachScoreCategory(ssSpotlightA, scSpotlightMusic);
            await _competitionScoreSetRepository.AttachScoreCategory(ssSpotlightA, scSpotlightStepping);
            await _competitionScoreSetRepository.AttachScoreCategory(ssSpotlightA, scSpotlightPresentation);
            await _competitionScoreSetRepository.AttachScoreCategory(ssSpotlightA, scSpotlightBuzzFactor);
            await _competitionScoreSetRepository.AttachScoreCategory(ssSpotlightA, scSpotlightCharacters);

            await _competitionScoreSetRepository.AttachScoreCategory(ssSpotlightB, scSpotlightDanceTechnique);
            await _competitionScoreSetRepository.AttachScoreCategory(ssSpotlightB, scSpotlightSwordHandling);
            await _competitionScoreSetRepository.AttachScoreCategory(ssSpotlightB, scSpotlightPresentation);
            await _competitionScoreSetRepository.AttachScoreCategory(ssSpotlightB, scSpotlightBuzzFactor);
            await _competitionScoreSetRepository.AttachScoreCategory(ssSpotlightB, scSpotlightCharacters);

            //Traditional
            //Pre 2016
            //scoreSetRepo.AttachScoreCategory(ssTraditionalFull, scTraditionalBuzzFactor);
            //scoreSetRepo.AttachScoreCategory(ssTraditionalFull, scTraditionalStepping);
            //scoreSetRepo.AttachScoreCategory(ssTraditionalFull, scTraditionalStyle);

            //Main
            await _competitionScoreSetRepository.AttachScoreCategory(ssTraditionalA, scTraditionalMusic);
            await _competitionScoreSetRepository.AttachScoreCategory(ssTraditionalA, scTraditionalStepping);
            await _competitionScoreSetRepository.AttachScoreCategory(ssTraditionalA, scTraditionalPresentation);
            await _competitionScoreSetRepository.AttachScoreCategory(ssTraditionalA, scTraditionalBuzzFactor);
            await _competitionScoreSetRepository.AttachScoreCategory(ssTraditionalA, scTraditionalCharacters);

            await _competitionScoreSetRepository.AttachScoreCategory(ssTraditionalB, scTraditionalDanceTechnique);
            await _competitionScoreSetRepository.AttachScoreCategory(ssTraditionalB, scTraditionalSwordHandling);
            await _competitionScoreSetRepository.AttachScoreCategory(ssTraditionalB, scTraditionalPresentation);
            await _competitionScoreSetRepository.AttachScoreCategory(ssTraditionalB, scTraditionalBuzzFactor);
            await _competitionScoreSetRepository.AttachScoreCategory(ssTraditionalB, scTraditionalCharacters);

            await _competitionScoreSetRepository.AttachScoreCategory(ssTraditionalC1, scTraditionalAlignment);
            await _competitionScoreSetRepository.AttachScoreCategory(ssTraditionalC2, scTraditionalAlignment);

            //Derty
            await _competitionScoreSetRepository.AttachScoreCategory(ssDertyFull, scDertyStepping);
            await _competitionScoreSetRepository.AttachScoreCategory(ssDertyFull, scDertyFigures);
            await _competitionScoreSetRepository.AttachScoreCategory(ssDertyFull, scDertyStyle);
            await _competitionScoreSetRepository.AttachScoreCategory(ssDertyFull, scDertySwordHandling);
            await _competitionScoreSetRepository.AttachScoreCategory(ssDertyFull, scDertyTeamwork);


            //Veterans
            await _competitionScoreSetRepository.AttachScoreCategory(ssVeteransA, scVeteransMusic);
            await _competitionScoreSetRepository.AttachScoreCategory(ssVeteransA, scVeteransStepping);
            await _competitionScoreSetRepository.AttachScoreCategory(ssVeteransA, scVeteransPresentation);
            await _competitionScoreSetRepository.AttachScoreCategory(ssVeteransA, scVeteransBuzzFactor);
            await _competitionScoreSetRepository.AttachScoreCategory(ssVeteransA, scVeteransCharacters);

            await _competitionScoreSetRepository.AttachScoreCategory(ssVeteransB, scVeteransDanceTechnique);
            await _competitionScoreSetRepository.AttachScoreCategory(ssVeteransB, scVeteransSwordHandling);
            await _competitionScoreSetRepository.AttachScoreCategory(ssVeteransB, scVeteransPresentation);
            await _competitionScoreSetRepository.AttachScoreCategory(ssVeteransB, scVeteransBuzzFactor);
            await _competitionScoreSetRepository.AttachScoreCategory(ssVeteransB, scVeteransCharacters);

            #endregion

            //Main Comp ID
            EventSetting mainCompId = new EventSetting();
            mainCompId.EventId = eventId;
            mainCompId.Ref = EventSettingType.MAINCOMP_COMPETITION_ID.ToString();
            mainCompId.Name = "Main Competition ID";
            mainCompId.Value = mainCompetition.Id.ToString();
            mainCompId.AccessToken = null;
            eventSettingsToApply.Add(mainCompId);

            //Main Comp Characters Score Category
            EventSetting mainCompCharactersScoreCategoryId = new EventSetting();
            mainCompCharactersScoreCategoryId.EventId = eventId;
            mainCompCharactersScoreCategoryId.Ref = EventSettingType.MAINCOMP_CHARACTERS_SCORECATEGORY_ID.ToString();
            mainCompCharactersScoreCategoryId.Name = "Score Category ID - Main Comp Characters";
            mainCompCharactersScoreCategoryId.Value = scMainCharacters.Id.ToString();
            mainCompCharactersScoreCategoryId.AccessToken = null;
            eventSettingsToApply.Add(mainCompCharactersScoreCategoryId);

            //Main Comp Buzz Score Category
            EventSetting mainCompBuzzScoreCategoryId = new EventSetting();
            mainCompBuzzScoreCategoryId.EventId = eventId;
            mainCompBuzzScoreCategoryId.Ref = EventSettingType.MAINCOMP_BUZZ_SCORECATEGORY_ID.ToString();
            mainCompBuzzScoreCategoryId.Name = "Score Category ID - Main Comp Buzz";
            mainCompBuzzScoreCategoryId.Value = scMainBuzzFactor.Id.ToString();
            mainCompBuzzScoreCategoryId.AccessToken = null;
            eventSettingsToApply.Add(mainCompBuzzScoreCategoryId);

            //Main Comp Music Score Category
            EventSetting mainCompMusicScoreCategoryId = new EventSetting();
            mainCompMusicScoreCategoryId.EventId = eventId;
            mainCompMusicScoreCategoryId.Ref = EventSettingType.MAINCOMP_MUSIC_SCORECATEGORY_ID.ToString();
            mainCompMusicScoreCategoryId.Name = "Score Category ID - Main Comp Music";
            mainCompMusicScoreCategoryId.Value = scMainMusic.Id.ToString();
            mainCompMusicScoreCategoryId.AccessToken = null;
            eventSettingsToApply.Add(mainCompMusicScoreCategoryId);

            //Results Setting
            EventSetting resultsPublished = new EventSetting();
            resultsPublished.EventId = eventId;
            resultsPublished.Ref = EventSettingType.RESULTS_PUBLISHED.ToString();
            resultsPublished.Name = "Results Published To Public";
            resultsPublished.Value = "false";
            resultsPublished.AccessToken = null;
            eventSettingsToApply.Add(resultsPublished);

            return eventSettingsToApply;
        }

        public async Task<List<EventSetting>> ApplyClassicSwordDancingTemplate(int eventId)
        {
            var eventSettingsToApply = new List<EventSetting>();

            #region Competitions

            //MainCompetition
            Competition mainCompetition = new Competition();
            mainCompetition.Name = "Main Competition";
            mainCompetition.CompetitionDescription = "Main Competition";
            mainCompetition.TeamEntryFee = 0;
            mainCompetition.EventId = eventId;
            mainCompetition.JudgeRequirementPerVenue = 2;
            mainCompetition.FlowState = CompetitionFlowState.New;
            mainCompetition.AccessToken = null;
            mainCompetition.IsDeleted = false;
            mainCompetition.AllowAdHocDanceAddition = false;
            mainCompetition.ResultsAreCollated = true;
            mainCompetition = await _competitionRepository.Add(mainCompetition);

            //Traditional Competition
            Competition traditionalCompetition = new Competition();
            traditionalCompetition.Name = "Traditional Competition";
            traditionalCompetition.CompetitionDescription = "Traditional Competition";
            traditionalCompetition.TeamEntryFee = 0;
            traditionalCompetition.EventId = eventId;
            traditionalCompetition.JudgeRequirementPerVenue = 4;
            traditionalCompetition.FlowState = CompetitionFlowState.New;
            traditionalCompetition.AccessToken = null;
            traditionalCompetition.IsDeleted = false;
            traditionalCompetition.AllowAdHocDanceAddition = true;
            traditionalCompetition.ResultsAreCollated = false;
            traditionalCompetition = await _competitionRepository.Add(traditionalCompetition);

            #endregion

            #region Bind Competitions As Activities

            Activity activity1 = new Activity();
            activity1.EventId = eventId;
            activity1.CompetitionId = mainCompetition.Id;
            activity1.Title = mainCompetition.Name;
            activity1.Price = 0;
            activity1.IsDefault = true;
            activity1.AudienceTypeId = (int)ActivityAudienceType.TEAM;
            activity1 = await _activityRepository.Add(activity1);

            Activity activity2 = new Activity();
            activity2.EventId = eventId;
            activity2.CompetitionId = traditionalCompetition.Id;
            activity2.Title = traditionalCompetition.Name;
            activity2.Price = 0;
            activity2.IsDefault = false;
            activity2.AudienceTypeId = (int)ActivityAudienceType.TEAM;
            activity2 = await _activityRepository.Add(activity2);

            #endregion 

            #region Competition Entry Attributes

            CompetitionEntryAttribute ceaVeterans = new CompetitionEntryAttribute();
            ceaVeterans.Name = "Veterans";
            ceaVeterans.Tag = "[V]";
            ceaVeterans.CompetitionAppliesToId = mainCompetition.Id;
            ceaVeterans.AccessToken = null;
            ceaVeterans = await _competitionEntryAttributeRepository.Add(ceaVeterans);

            CompetitionEntryAttribute ceaSduAlt = new CompetitionEntryAttribute();
            ceaSduAlt.Name = "SDU Alternative";
            ceaSduAlt.Tag = "[A]";
            ceaSduAlt.CompetitionAppliesToId = mainCompetition.Id;
            ceaSduAlt.AccessToken = null;
            ceaSduAlt = await _competitionEntryAttributeRepository.Add(ceaSduAlt);

            CompetitionEntryAttribute ceaBestNewcomer = new CompetitionEntryAttribute();
            ceaBestNewcomer.Name = "Best Newcomer";
            ceaBestNewcomer.Tag = "[N]";
            ceaBestNewcomer.CompetitionAppliesToId = mainCompetition.Id;
            ceaBestNewcomer.AccessToken = null;
            ceaBestNewcomer = await _competitionEntryAttributeRepository.Add(ceaBestNewcomer);

            CompetitionEntryAttribute premier = new CompetitionEntryAttribute();
            premier.Name = "Premier  Class";
            premier.Tag = "[P]";
            premier.CompetitionAppliesToId = mainCompetition.Id;
            premier.AccessToken = null;
            premier = await _competitionEntryAttributeRepository.Add(premier);

            CompetitionEntryAttribute champtionship = new CompetitionEntryAttribute();
            champtionship.Name = "Championship  Class";
            champtionship.Tag = "[C]";
            champtionship.CompetitionAppliesToId = mainCompetition.Id;
            champtionship.AccessToken = null;
            champtionship = await _competitionEntryAttributeRepository.Add(champtionship);

            CompetitionEntryAttribute open = new CompetitionEntryAttribute();
            open.Name = "Open  Class";
            open.Tag = "[O]";
            open.CompetitionAppliesToId = mainCompetition.Id;
            open.AccessToken = null;
            open = await _competitionEntryAttributeRepository.Add(open);

            #endregion 

            #region Score Sets

            ScoreSet ssMainA = new ScoreSet();
            ssMainA.Name = "Main Competition-SetA";
            ssMainA.Description = "Main Competition-SetA";
            ssMainA.CompetitionId = mainCompetition.Id;
            ssMainA.AccessToken = null;
            ssMainA.IsDeleted = false;
            ssMainA = await _competitionScoreSetRepository.Add(ssMainA);

            ScoreSet ssMainB = new ScoreSet();
            ssMainB.Name = "Main Competition-SetB";
            ssMainB.Description = "Main Competition-SetB";
            ssMainB.CompetitionId = mainCompetition.Id;
            ssMainB.AccessToken = null;
            ssMainB.IsDeleted = false;
            ssMainB = await _competitionScoreSetRepository.Add(ssMainB);

            ScoreSet ssTraditionalA = new ScoreSet();
            ssTraditionalA.Name = "Traditional Competition-SetA";
            ssTraditionalA.Description = "Traditional Competition-SetA";
            ssTraditionalA.CompetitionId = traditionalCompetition.Id;
            ssTraditionalA.AccessToken = null;
            ssTraditionalA.IsDeleted = false;
            ssTraditionalA = await _competitionScoreSetRepository.Add(ssTraditionalA);

            ScoreSet ssTraditionalB = new ScoreSet();
            ssTraditionalB.Name = "Traditional Competition-SetB";
            ssTraditionalB.Description = "Traditional Competition-SetB";
            ssTraditionalB.CompetitionId = traditionalCompetition.Id;
            ssTraditionalB.AccessToken = null;
            ssTraditionalB.IsDeleted = false;
            ssTraditionalB = await _competitionScoreSetRepository.Add(ssTraditionalB);

            ScoreSet ssTraditionalC1 = new ScoreSet();
            ssTraditionalC1.Name = "Traditional Competition-SetC1";
            ssTraditionalC1.Description = "Traditional Competition-SetC1";
            ssTraditionalC1.CompetitionId = traditionalCompetition.Id;
            ssTraditionalC1.AccessToken = null;
            ssTraditionalC1.IsDeleted = false;
            ssTraditionalC1 = await _competitionScoreSetRepository.Add(ssTraditionalC1);

            //C2 - required to match up with 4 judges
            ScoreSet ssTraditionalC2 = new ScoreSet();
            ssTraditionalC2.Name = "Traditional Competition-SetC2";
            ssTraditionalC2.Description = "Traditional Competition-SetC2";
            ssTraditionalC2.CompetitionId = traditionalCompetition.Id;
            ssTraditionalC2.AccessToken = null;
            ssTraditionalC2.IsDeleted = false;
            ssTraditionalC2 = await _competitionScoreSetRepository.Add(ssTraditionalC2);

            #endregion

            #region Score Categories

            //Main Competition

            #region Main Competition

            ScoreCategory scMainMusic = new ScoreCategory();
            scMainMusic.CompetitionAppliesToId = mainCompetition.Id;
            scMainMusic.Name = "Music [M]";
            scMainMusic.Description = "[Main] Music";
            scMainMusic.Tag = "MU";
            scMainMusic.MaxMarks = 15;
            scMainMusic.SortOrder = 10;
            scMainMusic.AccessToken = null;
            scMainMusic = await _competitionScoreCategoryRepository.Add(scMainMusic);

            ScoreCategory scMainStepping = new ScoreCategory();
            scMainStepping.CompetitionAppliesToId = mainCompetition.Id;
            scMainStepping.Name = "Stepping [M]";
            scMainStepping.Description = "[Main] Stepping";
            scMainStepping.Tag = "ST";
            scMainStepping.MaxMarks = 15;
            scMainStepping.SortOrder = 20;
            scMainStepping.AccessToken = null;
            scMainStepping = await _competitionScoreCategoryRepository.Add(scMainStepping);

            ScoreCategory scMainSwordHandling = new ScoreCategory();
            scMainSwordHandling.CompetitionAppliesToId = mainCompetition.Id;
            scMainSwordHandling.Name = "Sword Handling [M]";
            scMainSwordHandling.Description = "[Main] Sword Handling";
            scMainSwordHandling.Tag = "SH";
            scMainSwordHandling.MaxMarks = 15;
            scMainSwordHandling.SortOrder = 30;
            scMainSwordHandling.AccessToken = null;
            scMainSwordHandling = await _competitionScoreCategoryRepository.Add(scMainSwordHandling);

            ScoreCategory scMainDanceTechnique = new ScoreCategory();
            scMainDanceTechnique.CompetitionAppliesToId = mainCompetition.Id;
            scMainDanceTechnique.Name = "Dance Technique [M] ";
            scMainDanceTechnique.Description = "[Main] Dance Technique";
            scMainDanceTechnique.Tag = "DT";
            scMainDanceTechnique.MaxMarks = 15;
            scMainDanceTechnique.SortOrder = 40;
            scMainDanceTechnique.AccessToken = null;
            scMainDanceTechnique = await _competitionScoreCategoryRepository.Add(scMainDanceTechnique);

            ScoreCategory scMainPresentation = new ScoreCategory();
            scMainPresentation.CompetitionAppliesToId = mainCompetition.Id;
            scMainPresentation.Name = "Presentation [M]";
            scMainPresentation.Description = "[Main] Presentation";
            scMainPresentation.Tag = "PR";
            scMainPresentation.MaxMarks = 15;
            scMainPresentation.SortOrder = 410;
            scMainPresentation.AccessToken = null;
            scMainPresentation = await _competitionScoreCategoryRepository.Add(scMainPresentation);

            ScoreCategory scMainBuzzFactor = new ScoreCategory();
            scMainBuzzFactor.CompetitionAppliesToId = mainCompetition.Id;
            scMainBuzzFactor.Name = "Buzz Factor [M]";
            scMainBuzzFactor.Description = "[Main] Buzz Factor";
            scMainBuzzFactor.Tag = "BZ";
            scMainBuzzFactor.MaxMarks = 15;
            scMainBuzzFactor.SortOrder = 420;
            scMainBuzzFactor.AccessToken = null;
            scMainBuzzFactor = await _competitionScoreCategoryRepository.Add(scMainBuzzFactor);

            ScoreCategory scMainCharacters = new ScoreCategory();
            scMainCharacters.CompetitionAppliesToId = mainCompetition.Id;
            scMainCharacters.Name = "Characters [M]";
            scMainCharacters.Description = "[Main] Characters";
            scMainCharacters.Tag = "CH";
            scMainCharacters.MaxMarks = 10;
            scMainCharacters.SortOrder = 900;
            scMainCharacters.AccessToken = null;
            scMainCharacters = await _competitionScoreCategoryRepository.Add(scMainCharacters);

            #endregion

            #region Traditional Competition

            ScoreCategory scTraditionalMusic = new ScoreCategory();
            scTraditionalMusic.CompetitionAppliesToId = traditionalCompetition.Id;
            scTraditionalMusic.Name = "Music [Tr]";
            scTraditionalMusic.Description = "[Trad] Music";
            scTraditionalMusic.Tag = "MU";
            scTraditionalMusic.MaxMarks = 15;
            scTraditionalMusic.SortOrder = 10;
            scTraditionalMusic.AccessToken = null;
            scTraditionalMusic = await _competitionScoreCategoryRepository.Add(scTraditionalMusic);

            ScoreCategory scTraditionalStepping = new ScoreCategory();
            scTraditionalStepping.CompetitionAppliesToId = traditionalCompetition.Id;
            scTraditionalStepping.Name = "Stepping [Tr]";
            scTraditionalStepping.Description = "[Trad] Stepping";
            scTraditionalStepping.Tag = "ST";
            scTraditionalStepping.MaxMarks = 15;
            scTraditionalStepping.SortOrder = 20;
            scTraditionalStepping.AccessToken = null;
            scTraditionalStepping = await _competitionScoreCategoryRepository.Add(scTraditionalStepping);

            ScoreCategory scTraditionalSwordHandling = new ScoreCategory();
            scTraditionalSwordHandling.CompetitionAppliesToId = traditionalCompetition.Id;
            scTraditionalSwordHandling.Name = "Sword Handling [Tr]";
            scTraditionalSwordHandling.Description = "[Trad] Sword Handling";
            scTraditionalSwordHandling.Tag = "SH";
            scTraditionalSwordHandling.MaxMarks = 15;
            scTraditionalSwordHandling.SortOrder = 30;
            scTraditionalSwordHandling.AccessToken = null;
            scTraditionalSwordHandling = await _competitionScoreCategoryRepository.Add(scTraditionalSwordHandling);

            ScoreCategory scTraditionalDanceTechnique = new ScoreCategory();
            scTraditionalDanceTechnique.CompetitionAppliesToId = traditionalCompetition.Id;
            scTraditionalDanceTechnique.Name = "Dance Technique [Tr] ";
            scTraditionalDanceTechnique.Description = "[Trad] Dance Technique";
            scTraditionalDanceTechnique.Tag = "DT";
            scTraditionalDanceTechnique.MaxMarks = 15;
            scTraditionalDanceTechnique.SortOrder = 40;
            scTraditionalDanceTechnique.AccessToken = null;
            scTraditionalDanceTechnique = await _competitionScoreCategoryRepository.Add(scTraditionalDanceTechnique);

            ScoreCategory scTraditionalPresentation = new ScoreCategory();
            scTraditionalPresentation.CompetitionAppliesToId = traditionalCompetition.Id;
            scTraditionalPresentation.Name = "Presentation [Tr]";
            scTraditionalPresentation.Description = "[Trad] Presentation";
            scTraditionalPresentation.Tag = "PR";
            scTraditionalPresentation.MaxMarks = 15;
            scTraditionalPresentation.SortOrder = 410;
            scTraditionalPresentation.AccessToken = null;
            scTraditionalPresentation = await _competitionScoreCategoryRepository.Add(scTraditionalPresentation);

            ScoreCategory scTraditionalBuzzFactor = new ScoreCategory();
            scTraditionalBuzzFactor.CompetitionAppliesToId = traditionalCompetition.Id;
            scTraditionalBuzzFactor.Name = "Buzz Factor [Tr]";
            scTraditionalBuzzFactor.Description = "[Trad] Buzz Factor";
            scTraditionalBuzzFactor.Tag = "BZ";
            scTraditionalBuzzFactor.MaxMarks = 15;
            scTraditionalBuzzFactor.SortOrder = 420;
            scTraditionalBuzzFactor.AccessToken = null;
            scTraditionalBuzzFactor = await _competitionScoreCategoryRepository.Add(scTraditionalBuzzFactor);

            ScoreCategory scTraditionalCharacters = new ScoreCategory();
            scTraditionalCharacters.CompetitionAppliesToId = traditionalCompetition.Id;
            scTraditionalCharacters.Name = "Characters [Tr]";
            scTraditionalCharacters.Description = "[Trad] Characters";
            scTraditionalCharacters.Tag = "CH";
            scTraditionalCharacters.MaxMarks = 10;
            scTraditionalCharacters.SortOrder = 900;
            scTraditionalCharacters.AccessToken = null;
            scTraditionalCharacters = await _competitionScoreCategoryRepository.Add(scTraditionalCharacters);

            ScoreCategory scTraditionalAlignment = new ScoreCategory();
            scTraditionalAlignment.CompetitionAppliesToId = traditionalCompetition.Id;
            scTraditionalAlignment.Name = "Alignment [Tr]";
            scTraditionalAlignment.Description = "[Trad] Alignment";
            scTraditionalAlignment.Tag = "AL";
            scTraditionalAlignment.MaxMarks = 100;
            scTraditionalAlignment.SortOrder = 1200;
            scTraditionalAlignment.AccessToken = null;
            scTraditionalAlignment = await _competitionScoreCategoryRepository.Add(scTraditionalAlignment);

            #endregion 

            #endregion

            #region Apply Categories To Sets

            //Main
            await _competitionScoreSetRepository.AttachScoreCategory(ssMainA, scMainMusic);
            await _competitionScoreSetRepository.AttachScoreCategory(ssMainA, scMainStepping);
            await _competitionScoreSetRepository.AttachScoreCategory(ssMainA, scMainPresentation);
            await _competitionScoreSetRepository.AttachScoreCategory(ssMainA, scMainBuzzFactor);
            await _competitionScoreSetRepository.AttachScoreCategory(ssMainA, scMainCharacters);

            await _competitionScoreSetRepository.AttachScoreCategory(ssMainB, scMainDanceTechnique);
            await _competitionScoreSetRepository.AttachScoreCategory(ssMainB, scMainSwordHandling);
            await _competitionScoreSetRepository.AttachScoreCategory(ssMainB, scMainPresentation);
            await _competitionScoreSetRepository.AttachScoreCategory(ssMainB, scMainBuzzFactor);
            await _competitionScoreSetRepository.AttachScoreCategory(ssMainB, scMainCharacters);

            

            //Traditional
            await _competitionScoreSetRepository.AttachScoreCategory(ssTraditionalA, scTraditionalMusic);
            await _competitionScoreSetRepository.AttachScoreCategory(ssTraditionalA, scTraditionalStepping);
            await _competitionScoreSetRepository.AttachScoreCategory(ssTraditionalA, scTraditionalPresentation);
            await _competitionScoreSetRepository.AttachScoreCategory(ssTraditionalA, scTraditionalBuzzFactor);
            await _competitionScoreSetRepository.AttachScoreCategory(ssTraditionalA, scTraditionalCharacters);

            await _competitionScoreSetRepository.AttachScoreCategory(ssTraditionalB, scTraditionalDanceTechnique);
            await _competitionScoreSetRepository.AttachScoreCategory(ssTraditionalB, scTraditionalSwordHandling);
            await _competitionScoreSetRepository.AttachScoreCategory(ssTraditionalB, scTraditionalPresentation);
            await _competitionScoreSetRepository.AttachScoreCategory(ssTraditionalB, scTraditionalBuzzFactor);
            await _competitionScoreSetRepository.AttachScoreCategory(ssTraditionalB, scTraditionalCharacters);

            await _competitionScoreSetRepository.AttachScoreCategory(ssTraditionalC1, scTraditionalAlignment);
            await _competitionScoreSetRepository.AttachScoreCategory(ssTraditionalC2, scTraditionalAlignment);

            #endregion

            //Main Comp ID
            EventSetting mainCompId = new EventSetting();
            mainCompId.EventId = eventId;
            mainCompId.Ref = EventSettingType.MAINCOMP_COMPETITION_ID.ToString();
            mainCompId.Name = "Main Competition ID";
            mainCompId.Value = mainCompetition.Id.ToString();
            mainCompId.AccessToken = null;
            eventSettingsToApply.Add(mainCompId);

            //Main Comp Characters Score Category
            EventSetting mainCompCharactersScoreCategoryId = new EventSetting();
            mainCompCharactersScoreCategoryId.EventId = eventId;
            mainCompCharactersScoreCategoryId.Ref = EventSettingType.MAINCOMP_CHARACTERS_SCORECATEGORY_ID.ToString();
            mainCompCharactersScoreCategoryId.Name = "Score Category ID - Main Comp Characters";
            mainCompCharactersScoreCategoryId.Value = scMainCharacters.Id.ToString();
            mainCompCharactersScoreCategoryId.AccessToken = null;
            eventSettingsToApply.Add(mainCompCharactersScoreCategoryId);

            //Main Comp Buzz Score Category
            EventSetting mainCompBuzzScoreCategoryId = new EventSetting();
            mainCompBuzzScoreCategoryId.EventId = eventId;
            mainCompBuzzScoreCategoryId.Ref = EventSettingType.MAINCOMP_BUZZ_SCORECATEGORY_ID.ToString();
            mainCompBuzzScoreCategoryId.Name = "Score Category ID - Main Comp Buzz";
            mainCompBuzzScoreCategoryId.Value = scMainBuzzFactor.Id.ToString();
            mainCompBuzzScoreCategoryId.AccessToken = null;
            eventSettingsToApply.Add(mainCompBuzzScoreCategoryId);

            //Main Comp Music Score Category
            EventSetting mainCompMusicScoreCategoryId = new EventSetting();
            mainCompMusicScoreCategoryId.EventId = eventId;
            mainCompMusicScoreCategoryId.Ref = EventSettingType.MAINCOMP_MUSIC_SCORECATEGORY_ID.ToString();
            mainCompMusicScoreCategoryId.Name = "Score Category ID - Main Comp Music";
            mainCompMusicScoreCategoryId.Value = scMainMusic.Id.ToString();
            mainCompMusicScoreCategoryId.AccessToken = null;
            eventSettingsToApply.Add(mainCompMusicScoreCategoryId);

            //Results Setting
            EventSetting resultsPublished = new EventSetting();
            resultsPublished.EventId = eventId;
            resultsPublished.Ref = EventSettingType.RESULTS_PUBLISHED.ToString();
            resultsPublished.Name = "Results Published To Public";
            resultsPublished.Value = "false";
            resultsPublished.AccessToken = null;
            eventSettingsToApply.Add(resultsPublished);

            return eventSettingsToApply;
        }
    }
}
