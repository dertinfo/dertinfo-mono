import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HighlightDirective } from './highlight.directive';
import { ImageCheckDirective } from './imagecheck.directive';
import { LoadingDirective } from './loading.directive';
import { MaintainOriginalDirective } from './maintain-original.directive';
import { RangeValidator } from './range-validator.directive';

@NgModule({
    imports: [
        CommonModule
    ],
    declarations: [
        HighlightDirective,
        ImageCheckDirective,
        LoadingDirective,
        MaintainOriginalDirective,
        RangeValidator
    ],
    exports: [
        HighlightDirective,
        ImageCheckDirective,
        LoadingDirective,
        MaintainOriginalDirective,
        RangeValidator
    ]
})
export class DirectivesModule { }
