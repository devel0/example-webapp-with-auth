import { emptyString } from "../../../utils/utils"
import { CompareOp, FieldKind, FilterNfo } from "../types/data-grid-types"
import { from } from 'linq-to-typescript'
import { pascalCase } from 'change-case'

export interface ColumnFilterNfo<T> {
    key: keyof T
    filterNfo: FilterNfo
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

            if (emptyString(columnFilter.filterNfo.filter)) continue;

            const fieldName = `${pascalCase(columnFilter.key as string)}`

            let dynFilter = columnFilter.filterNfo.filter

            if (columnFilter.kind != null && columnFilter.kind !== FieldKind.generic) {
                switch (columnFilter.filterNfo.op) {
                    case CompareOp.lessThan: dynFilter = `< ${dynFilter}`; break
                    case CompareOp.lessThansOrEquals: dynFilter = `<= ${dynFilter}`; break
                    case CompareOp.Equals: dynFilter = `== ${dynFilter}`; break
                    case CompareOp.greatThanOrEquals: dynFilter = `>= ${dynFilter}`; break
                    case CompareOp.greatThan: dynFilter = `> ${dynFilter}`; break
                }
            }

            // date time offset filters
            if (columnFilter.kind === FieldKind.dateTimeOffset) {
                const opStr = dynFilter.split(' ')[0]
                const dtStr = dynFilter.split(' ')[1].replace(/\"/g, '').trim()
                const dt = new Date(dtStr)

                if (dynFilter.startsWith('==')) {
                    // '== 2000-01-06T23:00:00+00:00'
                    // = dt need to be converted >= dt and < dt+1day                                        
                    const dtMore1Day = new Date(dt.valueOf() + 24 * 60 * 60 * 1000)

                    filters.push(`( ${fieldName} >= "${dt.toISOString()}" ) && ( ${fieldName} < "${dtMore1Day.toISOString()}" )`)
                }
                else
                    filters.push(`${fieldName} ${opStr} "${new Date(dynFilter).toISOString()}"`)
            }

            else if (columnFilter.kind === FieldKind.numeric) {
                filters.push(`${fieldName} ${dynFilter}`)
            }

            // generic filters
            else {
                const ss_searchTerm =
                    from(dynFilter.split(' ')).select(w => w.trim()).where(r => r.length > 0).toArray()

                for (let ssIdx = 0; ssIdx < ss_searchTerm.length; ++ssIdx) {
                    let searchTerm = ss_searchTerm[ssIdx]

                    let filter = ``

                    if (columnFilter.preProcessField != null)
                        filter = `${columnFilter.preProcessField}(${fieldName})`
                    else
                        filter = `${fieldName}`

                    if (columnFilter.filterCaseSensitive !== true) {
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