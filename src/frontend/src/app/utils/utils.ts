export function delayAsync(ms: number) {
    return new Promise(resolve => setTimeout(resolve, ms));
}

/** true if given string is null or whitespace empty */
export const emptyString = (str: string | null | undefined) => {
    if (str == null) return true
    return str.trim().length === 0
}

export const sizeHumanizer = (bytes: number) => {
    const units = ["B", "KB", "MB", "GB", "TB"];
    let i = 0;
    while (bytes >= 1024 && i < units.length - 1) {
        bytes /= 1024;
        i++;
    }
    return `${bytes.toFixed(2)} ${units[i]}`;
}

/**
 * build type safe nested field path
 * Usage example: pathBuilder<Place>()('contact', 'name')
 */
export function pathBuilder<T>() {
    return <
        K1 extends keyof T,
        K2 extends keyof NonNullable<T[K1]>,
        K3 extends keyof NonNullable<NonNullable<T[K1]>[K2]>,
        K4 extends keyof NonNullable<NonNullable<NonNullable<T[K1]>[K2]>[K3]>
    >(p: K1, p2?: K2, p3?: K3, p4?: K4) => {
        let res = String(p)
        if (p2) { res += "." + String(p2) }
        if (p3) { res += "." + String(p3) }
        if (p4) { res += "." + String(p4) }
        return res
    }
}

/**
 * retrieve nested field value of an object by given path
 */
export const nestedField = (obj: any, path: string) => {
    if (obj == null) return undefined

    const ss = path.split('.')

    let res = obj[ss[0]]

    for (let i = 1; i < ss.length; ++i) {
        if (!res) return undefined
        res = res[ss[i]]
    }

    return res
}

export function nameof<T>(key: keyof T, instance?: T): keyof T {
    return key;
}