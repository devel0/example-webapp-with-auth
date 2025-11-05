import { pascalCase } from "change-case";
import { from } from "linq-to-typescript";
import { emptyString, isGuid } from "../../utils/utils";
import { FieldKind } from "./DataGridTypes";

export interface ColumnFilterNfo<T> {
    key: keyof T
    value: string
    filterCaseSensitive?: boolean
    kind?: FieldKind,
    preProcessField?: string
}

/** generate filter expression using dynFilter and/or per column columnFilters */
export function buildGenericDynFilter<T>(props: {
    columnFilters: ColumnFilterNfo<T>[]
}) {
    const filters: string[] = []

    const { columnFilters } = props

    //-------------------------------------------
    // columnFilters

    if (columnFilters != null) {
        for (let cfi = 0; cfi < columnFilters.length; ++cfi) {
            const columnFilter = columnFilters[cfi]

            if (emptyString(columnFilter.value)) continue;

            const fieldName = `${pascalCase(columnFilter.key as string)}`

            // date time offset filters
            if (columnFilter.kind === FieldKind.dateTimeOffset) {
                if (columnFilter.value.startsWith('==')) {
                    // '== 2000-01-06T23:00:00+00:00'
                    // = dt need to be converted >= dt and < dt+1day                    
                    const dtStr = columnFilter.value.split(' ')[1].replace(/\"/g, '').trim()
                    const dt = new Date(dtStr)
                    const dtMore1Day = new Date(dt.valueOf() + 24 * 60 * 60 * 1000)

                    filters.push(`( ${fieldName} >= "${dt.toISOString()}" ) && ( ${fieldName} < "${dtMore1Day.toISOString()}" )`)
                }
                else
                    filters.push(`${fieldName} ${columnFilter.value}`)
            }

            else if (columnFilter.kind === FieldKind.numeric) {
                filters.push(`${fieldName} ${columnFilter.value}`)
            }

            // generic filters
            else {
                const ss_searchTerm =
                    from(columnFilter.value.split(' ')).select(w => w.trim()).where(r => r.length > 0).toArray()

                for (let ssIdx = 0; ssIdx < ss_searchTerm.length; ++ssIdx) {
                    let searchTerm = ss_searchTerm[ssIdx]

                    let filter = ``

                    if (columnFilter.preProcessField != null)
                        filter = `${columnFilter.preProcessField}(${fieldName})`
                    else
                        filter = `${fieldName}`

                    if (columnFilter.filterCaseSensitive === true) {
                        filter += ".ToLower()"
                        searchTerm = searchTerm.toLowerCase()
                    }

                    filter += `.Contains("${searchTerm}")`

                    filters.push(filter)
                }
            }
        }
    }

    return from(filters).select(ff => `( ${ff} )`).toArray().join(' && ')
}