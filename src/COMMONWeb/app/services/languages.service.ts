/// <reference types="angular-cookies" />
/// <reference types="angular-translate" />

import { Languages } from "../classes/languages";

export class LanguagesService {
    private static languages: Languages;

    constructor(private $translate: ng.translate.ITranslateService, private $cookies: ng.cookies.ICookiesService, private $q: ng.IQService) {
        LanguagesService.languages = new Languages($translate, $cookies);

        // console.log("LanguagesService.constructor");
   }

    get(): ng.IPromise<Languages> {
        let d: ng.IDeferred<Languages> = this.$q.defer<Languages>();
        d.resolve(LanguagesService.languages);
        return d.promise;
    }

    public static Factory(): Function {
        let factory = ($translate: ng.translate.ITranslateService, $cookies: ng.cookies.ICookiesService, $q: ng.IQService) => {
            return new LanguagesService($translate, $cookies, $q);
        }
        factory.$inject = ['$translate', '$cookies', '$q'];
        return factory;
    }
}
