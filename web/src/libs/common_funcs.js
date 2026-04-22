
import { getButtons } from '@/libs/util'
import { toFilterKey } from '@/libs/tools'
export default {
  install: (Vue, options) => {
    /**
     * @description 筛选按钮
     */
    Vue.prototype._filterButton = (buttonCode) => {
      let button = getButtons()
      let target = toFilterKey(button, 'buttonCode', buttonCode)
      return target.length > 0
    }
  }
}
