import { Pipe, PipeTransform } from '@angular/core';
import { Md5 } from 'ts-md5/dist/md5';

@Pipe({ name: 'gravitar' })
export class GravitarPipe implements PipeTransform {

    constructor() { }

    transform(value: string, users_name: string): string {

        let sanitisedGravitarEmail = '';

        if (value && value !== '') {
            sanitisedGravitarEmail = value.replace(' ', '').trim().toLowerCase();
        } else if (users_name && users_name !== '') {
            sanitisedGravitarEmail = users_name.replace(' ', '').trim().toLowerCase() + '@dertinfo.co.uk';
        } else {
            sanitisedGravitarEmail = '';
        }

        if (sanitisedGravitarEmail && sanitisedGravitarEmail !== '') {
            const hash = new Md5().appendStr(sanitisedGravitarEmail).end();
            return 'http://www.gravatar.com/avatar/' + hash + '?d=identicon';
        } else {
            return '';
        }
    }

}
