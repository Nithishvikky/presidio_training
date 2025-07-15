export function getFileTypeIcon(mimeType: string): string {
  const basePath = 'assets/icons/';

  if (!mimeType) return basePath + 'file-icon.png';
  if (mimeType.startsWith('image/')) return basePath + 'image-icon.png';
  if (mimeType.startsWith('video/')) return basePath + 'video-icon.png';
  if (mimeType.startsWith('audio/')) return basePath + 'audio-icon.png';
  if (mimeType === 'application/pdf') return basePath + 'pdf-icon.png';
  if (
    mimeType === 'application/vnd.openxmlformats-officedocument.wordprocessingml.document' ||
    mimeType.includes('word')
  ) {
    return basePath + 'word-icon.png';
  }

  if (
    mimeType === 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet' ||
    mimeType.includes('excel')
  ) {
    return basePath + 'excel-icon.png';
  }

  if (
    mimeType === 'application/vnd.openxmlformats-officedocument.presentationml.presentation' ||
    mimeType.includes('powerpoint')
  ) {
    return basePath + 'ppt-icon.png';
  }

  if (mimeType === 'text/plain') return basePath + 'text-icon.png';
  if (mimeType === 'text/csv') return basePath + 'csv-icon.png';
  if (
    mimeType === 'application/zip' ||
    mimeType.includes('compressed') ||
    mimeType.includes('x-rar') ||
    mimeType.includes('x-7z') ||
    mimeType.includes('x-tar') ||
    mimeType.includes('gzip')
  )
    return basePath + 'zip-icon.png';

  return basePath + 'file-icon.png';
}
