import { deepClone, copyValue, formateDateToString } from '@/libs/tools.js'
const BUTTONROLE = {
  WPDA_XG: 'WPDA_XG',
  WPDA_SC: 'WPDA_SC',
  WPDA_BJ: 'WPDA_BJ'
}
export default {
  data () {
    return {
      readonly: false,
      saveModal: true,
      buttonRole: BUTTONROLE,
      exportDatas: [], // cy 请求的导出数据
      // cy 导出的表字段
      exportColumns: [
        {
          title: '类别',
          key: 'catalogueItem'
        },
        {
          title: '编码',
          key: 'medicalItemCode'
        },
        {
          title: '名称',
          key: 'medicalItemName'
        },
        {
          title: '剂型',
          key: 'typeName'
        },
        {
          title: '剂量',
          key: 'doseMin'
        },
        {
          title: '中文单位',
          key: 'speUnitCHS'
        },
        {
          title: '英文单位',
          key: 'speUnitUS'
        },
        {
          title: '包装',
          key: 'packaging'
        },
        {
          title: '包装单位',
          key: 'packSpeUnitCHS'
        },
        {
          title: '规格',
          key: 'specifications'
        },
        {
          title: '规格单位',
          key: 'specSpeUnitCHS'
        },
        {
          title: '件比',
          key: 'medicalThan'
        },
        {
          title: '厂家',
          key: 'manufacturer'
        },
      ]
    }
  },
  methods: {
    // 查看，编辑，删除，调价
    showDetail (id, name) {
      let childComponent = this.$refs['item-file-template']
      childComponent.itemId = id
      this.itemId = id
      this.addFlag = true
      this.title = '查看'
      this.editLock = true
      this.detailTitle = name
      // this.lock = false
      this.readonly = true
      this.saveModal = false
      this.getItem()
    },
    // cy:因合并了查看详情和编辑功能
    showEdit (obj) {
      this.saveModal = true
      this.formValidate = JSON.parse(JSON.stringify(obj))
      delete this.formValidate._index
      delete this.formValidate._rowKey
      this.addFlag = true
      this.title = '修改'
      this.lock = true
      this.editLock = false
      // cy:详情页面合并编辑
      this.readonly = false
      // cy 尝试修改ruleValidate的生产厂家的必填
      if (this.ruleValidate.hasOwnProperty('manufacturer')) {
        this.ruleValidate.manufacturer[0].required = false
      }
    },

    showDel (item) {
      let childComponent = this.$refs['item-file-template']
      this.itemId = item.id
      childComponent.itemId = item.id
      childComponent.itemName = item.medicalItemName
      childComponent.delModal = true
      childComponent.delMessageContent = item.medFrequency < 6 ? `近半年内使用过，是否确认删除？` : `删除后不可恢复，您确定删除吗？`
    },
    cancel (name) {
      this.readonly && (this.readonly = false)
      this.editLock && (this.editLock = false)

      this.detailTitle = this.TYPENAME
      this.addFlag = false
      this.$refs[name].resetFields()
    },
    exportTb () {
      // console.log('asdasd', this.WAREHOUSEID)
      this.swsApi.swsGet(`Data/MedicalItemRecord/ExportList/${this.WAREHOUSEID}`)
        .then(res => {
          if (res.data.success) {
            this.exportDatas = res.data.result
            let columns = deepClone(this.exportColumns)
            columns = this.WAREHOUSEID === 1 ? columns : (() => { columns.splice(3, 4); return columns })()
            // console.log(columns)
            this.$refs['item-file-template'].$refs.wpda_table.exportCsv({
              filename: `${this.TYPENAME}档案${formateDateToString(new Date(), 'yyyyMMdd')}`,
              columns: columns,
              data: this.exportDatas
            })
          }
        })
        .catch(e => {
          this.$Message.error('请求错误，请稍后再试。 ', e)
        })
    },
    showAdd () {
      this.addFlag = true
      this.title = '新增'
      this.saveModal = true
      this.lock = false
      // this.editLock = false
      // cy 尝试修改ruleValidate的生产厂家的必填
      if (this.ruleValidate.hasOwnProperty('manufacturer')) {
        this.ruleValidate.manufacturer[0].required = true
      }
    },
    getCascader (data) {
      this.cascaderData = data
    },
    // 设置价格不为空
    setDefaultPrice (name) {
      let value = this.formValidate.medicalDrugExtension[name]
      !value &&
        value !== 0 &&
        (this.formValidate.medicalDrugExtension[name] = 0)
    },
    // 复制表单
    copyValue () {
      copyValue(this.formValidate, this.copiedFormValidate)
      this.$Message.success('已复制')
    },
    // 黏贴字段
    stickValue () {
      if (!Object.keys(this.copiedFormValidate).length) {
        this.$Message.warning('请先复制档案')
        return
      }
      this.formValidate = deepClone(this.copiedFormValidate)
    }
  }
}
