define(["require", "exports"], function (require, exports) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    var Utilities = /** @class */ (function () {
        function Utilities() {
        }
        Utilities.chunk = function (data, size) {
            var newArr = [];
            if (size > 0) {
                for (var i = 0; i < data.length; i += size) {
                    newArr.push(data.slice(i, i + size));
                }
            }
            return newArr;
        };
        Utilities.chunkToTupleOf2 = function (data) {
            var chunks = Utilities.chunk(data, 2);
            var arr = [];
            if (chunks && chunks.length > 0) {
                for (var i = 0; i < chunks[0].length; ++i)
                    arr.push([chunks[0][i], null]);
                for (var i = 0; i < chunks[1].length; ++i)
                    arr[1][i] = chunks[1][i];
            }
            return arr;
        };
        Utilities.toMidnight = function (date) {
            var y = date.getFullYear();
            var m = date.getMonth();
            var d = date.getDate();
            return new Date(y, m, d, 0, 0, 0, 0);
        };
        return Utilities;
    }());
    exports.Utilities = Utilities;
});
//# sourceMappingURL=utilities.js.map