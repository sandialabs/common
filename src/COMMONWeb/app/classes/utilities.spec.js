/// <reference types="jasmine" />
define(["require", "exports", "./utilities"], function (require, exports, utilities_1) {
    "use strict";
    Object.defineProperty(exports, "__esModule", { value: true });
    describe("Utilities", function () {
        describe("chunk", function () {
            it("Handle chunk of 1 of 1", function () {
                var array = [1];
                var result = utilities_1.Utilities.chunk(array, 1);
                expect(result).toEqual([[1]]);
            });
            it("Handle chunk of 2 of 1", function () {
                var array = [1];
                var result = utilities_1.Utilities.chunk(array, 2);
                expect(result).toEqual([[1]]);
            });
            it("Handle chunk of 10007 of 1", function () {
                var array = [1];
                var result = utilities_1.Utilities.chunk(array, 10007);
                expect(result).toEqual([[1]]);
            });
            it("Handle chunk of 0 of 1", function () {
                var array = [1];
                var result = utilities_1.Utilities.chunk(array, 0);
                expect(result).toEqual([]);
            });
            it("Handle chunk of 0 of 0", function () {
                var array = [];
                var result = utilities_1.Utilities.chunk(array, 0);
                expect(result).toEqual([]);
            });
            it("Handle chunk of 1 of 0", function () {
                var array = [];
                var result = utilities_1.Utilities.chunk(array, 1);
                expect(result).toEqual([]);
            });
            it("Handle chunk of 1 of 3", function () {
                var array = [1, 2, 3];
                var result = utilities_1.Utilities.chunk(array, 1);
                expect(result).toEqual([[1], [2], [3]]);
            });
            it("Handle chunk of 1 of 3", function () {
                var array = [1, 2, 3];
                var result = utilities_1.Utilities.chunk(array, 1);
                expect(result).toEqual([[1], [2], [3]]);
            });
            it("Handle chunk of 2 of 3", function () {
                var array = [1, 2, 3];
                var result = utilities_1.Utilities.chunk(array, 2);
                expect(result).toEqual([[1, 2], [3]]);
            });
            it("Handle chunk of 2 of 4", function () {
                var array = [1, 2, 3, 4];
                var result = utilities_1.Utilities.chunk(array, 2);
                expect(result).toEqual([[1, 2], [3, 4]]);
            });
            it("Handle chunk of 2 of 5", function () {
                var array = [1, 2, 3, 4, 5];
                var result = utilities_1.Utilities.chunk(array, 2);
                expect(result).toEqual([[1, 2], [3, 4], [5]]);
            });
            it("Handle chunk of 3 of 5", function () {
                var array = [1, 2, 3, 4, 5];
                var result = utilities_1.Utilities.chunk(array, 3);
                expect(result).toEqual([[1, 2, 3], [4, 5]]);
            });
            it("Handle chunk of 7 of 5", function () {
                var array = [1, 2, 3, 4, 5];
                var result = utilities_1.Utilities.chunk(array, 7);
                expect(result).toEqual([[1, 2, 3, 4, 5]]);
            });
            it("Handle string chunk of 3 of 5", function () {
                var array = ["1", "2", "3", "4", "5"];
                var result = utilities_1.Utilities.chunk(array, 3);
                expect(result).toEqual([["1", "2", "3"], ["4", "5"]]);
            });
        });
    });
});
//# sourceMappingURL=utilities.spec.js.map