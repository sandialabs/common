/// <reference types="angular-cookies" />
/// <reference types="angular-translate" />
define(["require", "exports"], function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    var Language = /** @class */ (function () {
        function Language(code, language) {
            this.code = code.toLowerCase();
            this.language = language;
        }
        return Language;
    }());
    exports.Language = Language;
    var Languages = /** @class */ (function () {
        function Languages($translate, $cookies) {
            this.$translate = $translate;
            this.$cookies = $cookies;
            this.languages = [new Language('en', 'English')];
            var language = "en";
            var language_cookie = $cookies.get("language");
            if (language_cookie !== undefined && language_cookie !== null)
                language = language_cookie;
            this.selectedLanguage = language;
            $translate.use(language);
            //moment.locale(language);
        }
        Languages.prototype.updateLanguages = function (data) {
            if (!data || !data.languages)
                return;
            for (var i = 0; i < data.languages.length; ++i) {
                if (data.languages[i].isEnabled === true) {
                    var language = new Language(data.languages[i].languageCode, data.languages[i].language);
                    var exists = false;
                    for (var j = 0; exists === false && j < this.languages.length; ++j) {
                        exists = this.languages[j].code === language.code;
                    }
                    if (exists === false)
                        this.languages.push(language);
                }
            }
        };
        Languages.prototype.use = function (language) {
            this.selectedLanguage = language;
            var now = new Date();
            var expiration = new Date(now.getFullYear(), now.getMonth() + 1, now.getDate());
            this.$cookies.put("language", language, { expires: expiration });
            this.$translate.use(language);
            //moment.locale(language);
        };
        return Languages;
    }());
    exports.Languages = Languages;
});
//# sourceMappingURL=languages.js.map