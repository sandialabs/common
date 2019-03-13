/// <reference types="angular-cookies" />
/// <reference types="angular-translate" />
define(["require", "exports", "../classes/languages"], function (require, exports, languages_1) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    var LanguagesService = /** @class */ (function () {
        function LanguagesService($translate, $cookies, $q) {
            this.$translate = $translate;
            this.$cookies = $cookies;
            this.$q = $q;
            LanguagesService.languages = new languages_1.Languages($translate, $cookies);
            // console.log("LanguagesService.constructor");
        }
        LanguagesService.prototype.get = function () {
            var d = this.$q.defer();
            d.resolve(LanguagesService.languages);
            return d.promise;
        };
        LanguagesService.Factory = function () {
            var factory = function ($translate, $cookies, $q) {
                return new LanguagesService($translate, $cookies, $q);
            };
            factory.$inject = ['$translate', '$cookies', '$q'];
            return factory;
        };
        return LanguagesService;
    }());
    exports.LanguagesService = LanguagesService;
});
//# sourceMappingURL=languages.service.js.map