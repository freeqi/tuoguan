<template>
  <div class="printbody">
    <div id="purchase_print" class="infor_cont">
      <div class="header">
        <h2 style="text-align: center;">{{`采购单${total_detail.purchaseNo || '合并明细表'}`}}</h2>
        <h4 style="text-align: left;">注:金额单位（元）</h4>
      </div>
      <table class="table-bordered">
        <tbody>
          <tr>
            <td width="50" class="text_center">序号</td>
            <td width="60" class="text_center">名称</td>
            <td width="60" class="text_center">规格</td>
            <td width="50" class="text_center">月均用量</td>
            <td width="50" class="text_center">当前库存</td>
            <td width="60" class="text_center">单位</td>
            <td width="50" class="text_center">采购数量</td>
            <td width="50" class="text_center">申请数量</td>
            <!-- <td width="60" class="text_center">建议采购单价</td> -->
            <td width="60" class="text_center">采购价</td>
            <td width="60" class="text_center">销售价</td>
            <!-- <td width="60" class="text_center">双控价</td> -->
            <td width="60" class="text_center">采购总金额</td>
            <td width="60" class="text_center">销售总金额</td>
            <td width="50" class="text_center" style="white-space: nowrap;">供应商</td>
            <td width="60" class="text_center">厂家</td>
            <td width="115" class="text_center">机构</td>
          </tr>
          <tr v-for="(item,index) in purchase_date" :key="index">
            <td>{{item.no}}</td>
            <td>{{item.medicalItemName}}</td>
            <td>{{item.minDose}}</td>
            <td>{{item.monthAverage}}</td>
            <td>{{item.currentInventory}}</td>
            <td>{{item.procurementUnitValue}}</td>
            <td>{{item.inQty}}</td>
            <td>{{item.approvalQty}}</td>
            <!-- <td>{{item.advicePrice}}</td> -->
            <td>{{item.inPrice}}</td>
            <td>{{item.salePrice}}</td>
            <!-- <td>{{item.dualPrice}}</td> -->
            <td>{{item.totalInPrice}}</td>
            <td>{{item.totalSalePrice}}</td>
            <td>{{item.supplierName}}</td>
            <td>{{item.manufacturer}}</td>
            <td>{{item.centerName}}</td>
          </tr>
          <tr>
            <td colspan="16">
              <p style="font-size:15px;">合计：</p>
              <p class="items-box">
                <span class="field">物品种类：</span>
                <span>{{total_detail.itemType}}</span>
                <span class="field">合计总数量：</span>
                <span>{{total_detail.totalQty || '0'}}件</span>
                <span class="field">采购总金额：</span>
                <span class="red">￥{{total_detail.actualPrice || '-'}}</span>
                <span class="field">销售总金额：</span>
                <span class="red">￥{{total_detail.salesTotalPrice || '-'}}</span>
              </p>
            </td>
          </tr>
        </tbody>
      </table>
    </div>
  </div>
</template>
<script>
import '@/view/print/print.less'
import '@/view/print/purchase_print.less'
export default {
  name: 'purchase_print',
  data () {
    return {
      flist: [],
      purchase_date: [],
      total_detail: {}
    }
  },
  created () {
    this.printMerge()
  },
  computed: {},
  methods: {
    printMerge () {
      let purchaseData = sessionStorage.getItem('purchaseData')
      let purchaseDetail = sessionStorage.getItem('purchaseDetail')
      this.purchase_date = JSON.parse(purchaseData)
      this.total_detail = JSON.parse(purchaseDetail)
      window.print()
    }
  }
}
</script>

<style lang="less" scoped="scoped">
#purchase_print {
  .header {
    text-align: center;
    margin-top: 15px;
  }
  .table-bordered > tbody > tr > td {
    vertical-align: middle !important;
    /*font-size: 14px !important;*/
    border: 1px solid #aaa !important;
    padding: 4px !important;
  }
  .text_center,
  .spTr td p {
    text-align: center;
  }
  .table-bordered {
    margin-bottom: 6px !important;
  }

  .items-box {
    line-height: 27px;
    font-size: 13px;
    .field {
      color: #666666;
      & + .red {
        color: #fc4b4b;
      }
    }
    span:not(.field) {
      font-weight: bold;
      margin-right: 50px;
    }
  }
}
</style>
