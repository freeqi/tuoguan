<template>
  <div id="repository">
    <info-template :CATALOG="catalog"></info-template>
  </div>
</template>

<script>
import infoTemplate from '@/components/information-template'
const CATALOG = 3
export default {
  data () {
    return {
      catalog: CATALOG
    }
  },
  components: {
    infoTemplate
  }
}
</script>

<style scoped lang="less">
#repository {
  position: relative;
  width: calc(~"100% - 20px");
  height: 100%;
  background: #ffffff;
}
</style>
