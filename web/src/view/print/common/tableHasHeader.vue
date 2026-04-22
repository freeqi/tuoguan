<template>
  <tbody>
    <!-- 对多表头梳理 -->
    <template v-if="multilevelHeader">
      <tr>
        <td
          v-for="item in columns"
          :rowspan="item.children && item.children.length ? 1 : 2"
          :key="item.key"
          :colspan="item.children && item.children.length"
        >{{item.title}}</td>
      </tr>
      <tr>
        <td v-for="item in secondHeader" :key="item.key">{{item.title}}</td>
      </tr>
    </template>
    <template v-else>
      <tr>
        <td v-for="item in columns" :key="item.key">{{item.title}}</td>
      </tr>
    </template>
    <template v-if="multilevelHeader">
      <tr v-for="item in datas" :key="item.itemName">
        <td v-for="field in flatColumns" :key="field.key">{{item[field.key]}}</td>
      </tr>
    </template>
    <template v-else>
      <tr v-for="item in datas" :key="item.itemName">
        <td v-for="field in columns" :key="field.key">{{item[field.key]}}</td>
      </tr>
    </template>
  </tbody>
</template>

<script>
export default {
  data () {
    return {
      multilevelHeader: false,
      secondHeader: [],
      flatColumns: []
    }
  },
  props: {
    columns: {
      default: []
    },
    datas: {
      default: []
    }
  },
  created () {
    for (let index = 0; index < this.columns.length; index++) {
      const element = this.columns[index]
      if (element.children && Array.isArray(element.children)) {
        let children = element.children
        this.multilevelHeader = true
        // 扁平化表头数组，绑定数据
        this.secondHeader = [...this.secondHeader, ...children]
        this.flatColumns = [...this.flatColumns, ...children]
      } else {
        this.flatColumns.push(element)
      }
    }
  },
  methods: {},
  components: {}
}
</script>

<style scoped lang="less">
</style>
