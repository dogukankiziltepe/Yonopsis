import { useCallback, useState } from 'react'
import type { AxiosResponse } from 'axios'
import type { WidgetFilter } from '@/components/dashboard/WidgetCard'
import type { ReportDateFilter } from '@/types/report'

export function useReportSection<T>(
  fetcher: (filter?: ReportDateFilter) => Promise<AxiosResponse<T>>,
  initial: T
) {
  const [data, setData] = useState<T>(initial)
  const [loading, setLoading] = useState(false)
  const [filter, setFilter] = useState<WidgetFilter>({ all: false })

  const load = useCallback(
    async (f: WidgetFilter = filter) => {
      setLoading(true)
      try {
        const res = await fetcher({ all: f.all, from: f.all ? undefined : f.from, to: f.all ? undefined : f.to })
        setData(res.data)
      } finally {
        setLoading(false)
      }
    },
    [fetcher, filter]
  )

  const changeFilter = (f: WidgetFilter) => {
    setFilter(f)
    load(f)
  }

  return { data, setData, loading, filter, load, changeFilter }
}

export function useReportRefresh<T>(fetcher: () => Promise<AxiosResponse<T>>, initial: T) {
  const [data, setData] = useState<T>(initial)
  const [loading, setLoading] = useState(false)

  const load = useCallback(async () => {
    setLoading(true)
    try {
      const res = await fetcher()
      setData(res.data)
    } finally {
      setLoading(false)
    }
  }, [fetcher])

  return { data, setData, loading, load }
}
