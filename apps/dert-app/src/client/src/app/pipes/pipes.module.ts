import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ImageSizePipe } from './imagesize.pipe';

@NgModule({
    imports: [
        CommonModule
    ],
    declarations: [
        ImageSizePipe
    ],
    exports: [
        ImageSizePipe
    ]
})
export class PipesModule { }
