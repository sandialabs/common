/// <reference types="jasmine" />

import { Utilities } from './utilities';

describe("Utilities", () => {
    describe("chunk", () => {
        it("Handle chunk of 1 of 1", () => {
            let array = [1];
            let result = Utilities.chunk(array, 1);
            expect(result).toEqual([[1]]);
        });

        it("Handle chunk of 2 of 1", () => {
            let array = [1];
            let result = Utilities.chunk(array, 2);
            expect(result).toEqual([[1]]);
        });

        it("Handle chunk of 10007 of 1", () => {
            let array = [1];
            let result = Utilities.chunk(array, 10007);
            expect(result).toEqual([[1]]);
        });

        it("Handle chunk of 0 of 1", () => {
            let array = [1];
            let result = Utilities.chunk(array, 0);
            expect(result).toEqual([]);
        });

        it("Handle chunk of 0 of 0", () => {
            let array = [];
            let result = Utilities.chunk(array, 0);
            expect(result).toEqual([]);
        });

        it("Handle chunk of 1 of 0", () => {
            let array = [];
            let result = Utilities.chunk(array, 1);
            expect(result).toEqual([]);
        });

        it("Handle chunk of 1 of 3", () => {
            let array = [1, 2, 3];
            let result = Utilities.chunk(array, 1);
            expect(result).toEqual([[1], [2], [3]]);
        });

        it("Handle chunk of 1 of 3", () => {
            let array = [1, 2, 3];
            let result = Utilities.chunk(array, 1);
            expect(result).toEqual([[1], [2], [3]]);
        });

        it("Handle chunk of 2 of 3", () => {
            let array = [1, 2, 3];
            let result = Utilities.chunk(array, 2);
            expect(result).toEqual([[1, 2], [3]]);
        });

        it("Handle chunk of 2 of 4", () => {
            let array = [1, 2, 3, 4];
            let result = Utilities.chunk(array, 2);
            expect(result).toEqual([[1, 2], [3, 4]]);
        });

        it("Handle chunk of 2 of 5", () => {
            let array = [1, 2, 3, 4, 5];
            let result = Utilities.chunk(array, 2);
            expect(result).toEqual([[1, 2], [3, 4], [5]]);
        });

        it("Handle chunk of 3 of 5", () => {
            let array = [1, 2, 3, 4, 5];
            let result = Utilities.chunk(array, 3);
            expect(result).toEqual([[1, 2, 3], [4, 5]]);
        });

        it("Handle chunk of 7 of 5", () => {
            let array = [1, 2, 3, 4, 5];
            let result = Utilities.chunk(array, 7);
            expect(result).toEqual([[1, 2, 3, 4, 5]]);
        });

        it("Handle string chunk of 3 of 5", () => {
            let array = ["1", "2", "3", "4", "5"];
            let result = Utilities.chunk(array, 3);
            expect(result).toEqual([["1", "2", "3"], ["4", "5"]]);
        });
    });
});
