import { toast } from 'sonner'

export const showError = (message?: string) =>
  toast.error(message ?? 'Bir hata oluştu.')

export const showSuccess = (message: string) =>
  toast.success(message)

export const showWarning = (message: string) =>
  toast.warning(message)

export const showInfo = (message: string) =>
  toast.info(message)

export const showPermissionError = () =>
  toast.error('Bu işlem için yetkiniz bulunmamaktadır.')

export const showNetworkError = () =>
  toast.error('Sunucuya bağlanılamadı. Lütfen internet bağlantınızı kontrol edin.')

export const showApiError = (error: unknown) => {
  if (typeof error === 'string') { showError(error); return }
  const err = error as { response?: { data?: { message?: string }; status?: number } }
  const message = err?.response?.data?.message
  if (message) { showError(message); return }
  showError()
}
