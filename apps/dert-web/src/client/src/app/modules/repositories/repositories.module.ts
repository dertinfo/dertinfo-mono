import { NgModule } from '@angular/core';
import { ClientSettingRepository } from './repositories/clientsetting.repository';
import { CompetitionRepository } from './repositories/competition.repository';
import { EventRepository } from './repositories/event.repository';
import { InvoiceRepository } from './repositories/invoice.repository';
import { ResultsAuthRepository } from './repositories/resultsauth.repository';
import { ShowcaseRepository } from './repositories/showcase.repository';
import { UserRepository } from './repositories/user.repository';
import { EventListCache } from './cache/eventlist.cache';
import { GroupListCache } from './cache/grouplist.cache';
import { RegistrationRepository } from './repositories/registration.repository';
import { GroupListRepository } from './cache/grouplist.repository';
import { EventListRepository } from './cache/eventlist.repository';

@NgModule({
    declarations: [

    ],
    exports: [

    ],
    providers: [
        ClientSettingRepository,
        CompetitionRepository,
        EventRepository,
        InvoiceRepository,
        ShowcaseRepository,
        UserRepository,
        ResultsAuthRepository,
        RegistrationRepository,
        GroupListCache,
        EventListCache,
        GroupListRepository,
        EventListRepository
    ]
})
export class RepositoriesModule {}
