/// <reference types="angular-cookies" />
/// <reference types="angular-translate" />

//declare var moment: any;
//let moment = require("../../lib/moment/min/moment-with-locales");

import { SystemConfiguration } from '../classes/systemconfiguration';

export class Language {
    public code: string;
    public language: string;

    constructor(code: string, language: string) {
        this.code = code.toLowerCase();
        this.language = language;
    }
}

export class Languages {
    public languages: Language[];
    public selectedLanguage: string;

    constructor(private $translate: ng.translate.ITranslateService, private $cookies: ng.cookies.ICookiesService) {
        this.languages = [new Language('en', 'English')];

        var language = "en";
        var language_cookie = $cookies.get("language");
        if (language_cookie !== undefined && language_cookie !== null)
            language = language_cookie;

        this.selectedLanguage = language;
        $translate.use(language);
        //moment.locale(language);
    }

    public updateLanguages(data: SystemConfiguration) {
        if (!data || !data.languages)
            return;

        for (var i = 0; i < data.languages.length; ++i) {
            if (data.languages[i].isEnabled === true) {
                var language = new Language(data.languages[i].languageCode, data.languages[i].language);

                var exists = false;
                for (var j = 0; exists === false && j < this.languages.length; ++j) {
                    exists = this.languages[j].code === language.code;
                }
                if(exists === false)
                    this.languages.push(language);
            }
        }
    }

    public use(language: string) {
        this.selectedLanguage = language;

        var now = new Date();
        var expiration = new Date(now.getFullYear(), now.getMonth() + 1, now.getDate());
        this.$cookies.put("language", language, { expires: expiration });
        this.$translate.use(language);

        //moment.locale(language);
    }
}