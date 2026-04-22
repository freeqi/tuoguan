export default class FullScreen {
  constructor () {
    this.prefixName = '' // 前缀
    this.fullScreenMethod = ''
    this.exitFunllScreenMethod = ''
    this.isFullscreenData = null
    this.isFullscreen()
  }
  isFullscreen (fn) {
    let fullscreenEnabled
    // 判断浏览器前缀
    if (document.fullscreenEnabled) {
      fullscreenEnabled = document.fullscreenEnabled
    } else if (document.webkitFullscreenEnabled) {
      fullscreenEnabled = document.webkitFullscreenEnabled
      this.prefixName = 'webkit'
    } else if (document.mozFullScreenEnabled) {
      fullscreenEnabled = document.mozFullScreenEnabled
      this.prefixName = 'moz'
    } else if (document.msFullscreenEnabled) {
      fullscreenEnabled = document.msFullscreenEnabled
      this.prefixName = 'ms'
    }
    this.fullScreenMethod =
      this.prefixName === ''
        ? 'requestFullscreen'
        : `${this.prefixName}RequestFullScreen`
    this.exitFunllScreenMethod =
      this.prefixName === ''
        ? 'exitFullscreen'
        : `${this.prefixName}ExitFullscreen`
    if (!fullscreenEnabled) {
      if (fn !== undefined) fn() // 执行不支持全屏的回调
      isFullscreenData = false
    }
  }

  fullScreen (element) {
    element[this.fullScreenMethod]()
  }

  exitFullScreen (element) {
    element[this.exitFunllScreenMethod]()
  }

  isElementFullScreen () {
    const fullscreenElement =
      document.fullscreenElement ||
      document.msFullscreenElement ||
      document.mozFullScreenElement ||
      document.webkitFullscreenElement
    if (fullscreenElement === null) {
      return false // 当前没有元素在全屏状态
    } else {
      return true // 有元素在全屏状态
    }
  }
}
