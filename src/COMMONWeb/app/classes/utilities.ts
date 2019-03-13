export abstract class Utilities {
    static chunk<T>(data: T[], size: number): T[][] {
        let newArr: T[][] = [];
        if (size > 0) {
            for (var i = 0; i < data.length; i += size) {
                newArr.push(data.slice(i, i + size));
            }
        }
        return newArr;
    }

    static chunkToTupleOf2<T>(data: T[]): [T, T][] {
        let chunks: T[][] = Utilities.chunk(data, 2);
        let arr: [T, T][] = [];

        if (chunks && chunks.length > 0) {
            for (var i = 0; i < chunks[0].length; ++i)
                arr.push([chunks[0][i], null]);
            for (var i = 0; i < chunks[1].length; ++i)
                arr[1][i] = chunks[1][i];
        }
        return arr;
    }

    static toMidnight(date: Date): Date {
        let y = date.getFullYear();
        let m = date.getMonth();
        let d = date.getDate();

        return new Date(y, m, d, 0, 0, 0, 0);
    }
}
