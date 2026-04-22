import CommonIcon from '_c/common-icon'
export default {
  components: {
    CommonIcon
  },
  methods: {
    showTitle (item) {
      return this.$config.useI18n ? this.$t(item.name) : ((item.meta && item.meta.title) || item.name)
    },
    showChildren (item) {
      return item.children && (item.children.length >= 1 || (item.meta && item.meta.showAlways)) // > change to >= 将一级菜单下只有一个二级菜单时，将二级菜单变成一级菜单的问题修复
    },
    getNameOrHref (item, children0) {
      return item.href ? `isTurnByHref_${item.href}` : (children0 ? item.children[0].name : item.name)
    }
  }
}
