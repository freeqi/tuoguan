<template>
    <div id="medical">
      <medical-record-list-temp :api="api" @on-change="changePatient">
        <div slot="content" style="height:95%;background: #fff;">
          <ul class="tab-panel">
            <li
              class="tab-panel-item"
              v-for="item in tabData"
              :key="item.id"
              @click="changeTab(item.id,item.index,item.name)"
              :class="tabCheckedInfo.id == item.id ? 'active' : ''"
            >{{item.name}}</li>
          </ul>
        
          <div class="table2">
            <h2>{{medicalCheckedInfo.hospitalName}}{{tabCheckedInfo.name}}{{`-${medicalCheckedInfo.patientName}`}}</h2>
            <div>
              <div class="btn-groups">
                <span>日期选择</span>
                <DatePicker
                  :value="dateRange"
                  placeholder="按创建时间筛选"
                  @on-change="changeDate"
                  type="datetimerange"
                  :options="dateRangeOptions"
                ></DatePicker>

                <Button type="primary" style="margin-left: 10px;" @click="exportTable">导出</Button>
              </div>
              <div class="table2-content" v-show="!table_detail_flag">
                <Table fixed :columns="table_columns" height="700" ref="table" :data="table_data" :loading="loading"></Table>
                <div class="pagination" v-if="dataCount > pageSize">
                  <Page
                    transfer
                    :total="dataCount"
                    :page-size="pageSize"
                    :current.sync="pageIndex"
                    @on-change="handleChangePage"
                  />
                </div>
              </div>
            </div>
            <div class="table_detail" v-show="table_detail_flag">
              <div class="head_content">
                <Button type="default" @click="hide" icon="md-undo">返回</Button>
                <p class="record">
                  {{tabCheckedInfo.name}}号：{{formDetail.prescriptionNo || formDetail.balanceNo}}
                  <span
                    v-if="formDetail.chargeStatus === '1'"
                    class="default"
                  >未收费</span>
                  <span v-else-if="formDetail.chargeStatus === '2'" class="status-text-success">已收费</span>
                  <span v-else-if="formDetail.chargeStatus === '3'" class="Success">部分退费</span>
                  <span v-else-if="formDetail.chargeStatus === '4'" class="error">全部退费</span>
                  <span v-else-if="formDetail.balanceState === 0" class="error">已退费</span>
                  <span v-else-if="formDetail.balanceState === 1" class="status-text-success">正常结算</span>
                  <span class="right button-group">
                    <!-- <Button type="primary" size="large" @click="">打印申请单</Button> -->
                  </span>
                </p>
                <p class="items-box">
                  <span class="field">划价时间：</span>
                  <span>{{formateDateToString(new Date(formDetail.balanceDate ? formDetail.balanceDate: formDetail.chargeDate), 'yyyy-MM-dd hh:mm:ss')}}</span>
                  <span class="field">姓名：</span>
                  <span>{{formDetail.patientName}}</span>
                  <span class="field">总金额：</span>
                  <span class="red">￥{{ formDetail.sumPrice|| prescriptionTotalMoney || '/'}}</span>
                </p>
              </div>
              <Divider></Divider>
              <div ref="detailTable">
                <Table
                  :columns="table_detail_columns"
                  :data="table_detail_data"
                  :loading="tableDetailLoading"
                  :height="tableHeight"
                ></Table>
              </div>
            </div>
          </div>
        </div>
      </medical-record-list-temp>
    </div>
  </template>
  
  <script>
  import { accAdd, subtract } from '@/libs/tools'
  import medicalRecordListTemp from '../medical/medical_record_list.vue'
  import columns from '../medical/columns.js'
  export default {
    mixins: [columns],
    components: {
      medicalRecordListTemp
    },
    data () {
      return {
        tableHeight: 0,
        dataCount: 0,
        pageSize: 9,
        pageIndex: 1,
  
        tableDetailLoading: false,
        table_detail_flag: false,
  
        prescriptionTotalMoney: 0,
  
        dateRange: [], // cy时间段筛选
        beginTime: '',
        endTime: '',
        dataFilterType: 1, // cy时间快捷筛选
        dataFilterTypeList: [
          { id: 0, label: '全部' },
          { id: 1, label: '近一个月' },
          { id: 2, label: '近三个月' }
        ],
        dateRangeOptions: {
          disabledDate (date) {
            return date && date.valueOf() > Date.now()
          }
        },
        medicalRecordChecked: {
          i: 0,
          id: '',
          recordName: ''
        },
        tabCheckedInfo: {
          id: 1,
          name: ''
        },
        tabData: [],
        // -------------------
        // cy被选择的机构、患者信息
        medicalCheckedInfo: {
          hospitalId: '',
          hospitalName: '',
          patientId: '',
          patientName: ''
        },
  
        table_columns: [],
        table_data: [],
        formDetail: [],
  
        loading: false,
        // pageSize: 12,
        // startPage: 1,
        // searchKey: '',
        api: 'Patients/Patients',
        // 病历类型，及选中id
        medicalType: 1,
        medicalTypeList: [
          {label: '病历首页', value: 1},
          {label: '专用病历', value: 2},
          {label: '门诊病历/透析记录单', value: 3},
          {label: '透析方案调整', value: 4},
          {label: '阶段小结', value: 5},
          {label: '护理评估记录', value: 6},
          {label: '健康宣教', value: 7},
          {label: '营养评估', value: 8}
        ],


    /*     //new
          //血常规
      BloodColumns: [
        { title: "送检日期", key: "CheckDate", minWidth: 110, align: "center" },
        {
          title: "检验机构",
          key: "Institutions",
          minWidth: 80,
          align: "center"
        },
        { title: "血红蛋白", key: "def9da4C8tZCS3874527185595404288", minWidth: 80, align: "center" },
        {
          title: "血小板计数",
          key: "def9da4ibYgdv3874529263319388160",
          minWidth: 80,
          align: "center"
        },
        {
          title: "嗜酸性粒细胞百分比",
          key: "def9da4RdCfZA3875603041596084224",
          minWidth: 80,
          align: "center"
        },
        {
          title: "嗜碱性粒细胞绝对值",
          key: "def9da43mFV7U3875603616194760705",
          minWidth: 80,
          align: "center"
        },
        {
          title: "嗜碱性粒细胞百分比",
          key: "BasophilsPercentage",
          minWidth: 80,
          align: "center"
        },
        {
          title: "平均红细胞血红蛋白浓度",
          key: "def9da4QOC5xh3875604003345797121",
          minWidth: 80,
          align: "center"
        },
        {
          title: "平均红细胞血红蛋白含量",
          key: "HemoglobinContent",
          minWidth: 80,
          align: "center"
        },
        {
          title: "平均红细胞体积",
          key: "def9da4RvJKAy3875604064809127936",
          minWidth: 80,
          align: "center"
        },
        {
          title: "淋巴细胞绝对数",
          key: "def9da4nRJ1kx3875604093632385024",
          minWidth: 80,
          align: "center"
        },
        {
          title: "淋巴细胞百分比",
          key: "def9da49sqkIL3875604127908237313",
          minWidth: 80,
          align: "center"
        },
        {
          title: "红细胞压积",
          key: "def9da4qmOOoY3875604159768170497",
          minWidth: 80,
          align: "center"
        },
        {
          title: "红细胞计数",
          key: "def9da4kxaRIZ3874530275979563009",
          minWidth: 80,
          align: "center"
        },
        {
          title: "红细胞分布宽度-标准差",
          key: "def9da4L0d7dd3875604224758910977",
          minWidth: 80,
          align: "center"
        },
        {
          title: "红细胞分布宽度-变异系数",
          key: "def9da4a7atDo3875604255633182721",
          minWidth: 80,
          align: "center"
        },
        {
          title: "单核细胞绝对数",
          key: "def9da4BmYiK23875604287883186176",
          minWidth: 80,
          align: "center"
        },
        {
          title: "单核细胞百分比",
          key: "def9da4RFcxZW3875604331394895872",
          minWidth: 80,
          align: "center"
        },
        {
          title: "大血小板比率",
          key: "MacroplateletRatio",
          minWidth: 80,
          align: "center"
        },
        {
          title: "白细胞计数",
          key: "def9da41m8bbO3875604396830232577",
          minWidth: 80,
          align: "center"
        },
        {
          title: "中性粒细胞绝对数",
          key: "def9da47itDDs3875604424244203521",
          minWidth: 80,
          align: "center"
        },
        {
          title: "中性粒细胞百分比",
          key: "def9da4gB323V3875604451255521281",
          minWidth: 80,
          align: "center"
        },
        {
          title: "血小板压积",
          key: "def9da4dC81Np3875604481165103105",
          minWidth: 80,
          align: "center"
        },
        {
          title: "血小板平均体积",
          key: "MeanPlateletVolume",
          minWidth: 80,
          align: "center"
        },
        {
          title: "血小板分布宽度",
          key: "def9da4AKIG4S3875604541231730688",
          minWidth: 80,
          align: "center"
        },
        {
          title: "嗜酸性粒细胞绝对值",
          key: "EosinophilsAbsolute",
          minWidth: 80,
          align: "center"
        },
        {
          title: "操作",
          key: "handle",
          minWidth: 80,
          align: "center",
          fixed: "right",
        //   slot: "handle"
          //					height: "64px",
        }
      ],
      // 电解质七项
      ElectrolyteColumns: [
        { title: "送检日期", key: "CheckDate", minWidth: 110, align: "center" },
        {
          title: "无机磷",
          key: "def9da4z9pWd53875151835488718848",
          minWidth: 80,
          align: "center"
        },
        { title: "镁", key: "def9da4FR0X5D3875609005187534849", minWidth: 80, align: "center" },
        { title: "钾", key: "def9da4skU2nI3875609030550491136", minWidth: 80, align: "center" },
        { title: "氯", key: "def9da43HxRk33875609057691832321", minWidth: 80, align: "center" },
        { title: "钠", key: "def9da4xtliaG3875609082748604417", minWidth: 80, align: "center" },
        { title: "钙", key: "def9da4vNNCsN3875609105230073857", minWidth: 80, align: "center" },
        {
          title: "二氧化碳",
          key: "def9da4zKA3UF3875609134812499969",
          minWidth: 80,
          align: "center"
        },
        // {
        //   title: "操作",
        //   key: "handle",
        //   minWidth: 80,
        //   align: "center",
        //   fixed: "right",
        //   render: (h, params) => {
        //     return h("div", [
        //       h(
        //         "Tooltip",
        //         {
        //           props: {
        //             placement: "top",
        //             content: "修改",
        //             transfer: true
        //           }
        //         },
        //         [
        //           h("Icon", {
        //             props: {
        //               type: "md-create"
        //             },
        //             style: {
        //               marginRight: "5px",
        //               fontSize: "18px",
        //               color: "#2D8CF0",
        //               cursor: "pointer"
        //             },
        //             on: {
        //               click: () => {
        //                 this.updateItem(params.row);
        //               }
        //             }
        //           })
        //         ]
        //       ),
        //       h(
        //         "Tooltip",
        //         {
        //           props: {
        //             placement: "top",
        //             content: "删除",
        //             transfer: true
        //           }
        //         },
        //         [
        //           h("Icon", {
        //             props: { type: "md-close" },
        //             style: {
        //               fontSize: "18px",
        //               color: "red",
        //               cursor: "pointer"
        //             },
        //             on: {
        //               click: () => {
        //                 this.deleteItem(params.row.Id);
        //               }
        //             }
        //           })
        //         ]
        //       )
        //     ]);
        //   }
        // }
      ],
      // 矿物质及骨代谢
      MineralsColumns: [
        { title: "送检日期", key: "CheckDate", minWidth: 110, align: "center" },
        {
          title: "检验机构",
          key: "Institutions",
          minWidth: 80,
          align: "center"
        },
        {
          title: "甲状旁腺激素",
          key: "ParathyroidHormone",
          minWidth: 80,
          align: "center"
        },
        {
          title: "骨型碱性磷酸酶",
          key: "BoneAlkalinePhosphatase",
          minWidth: 80,
          align: "center"
        },
        {
          title: "25-羟维生素D3",
          key: "Hydroxyvitamin",
          minWidth: 80,
          align: "center"
        },
        { title: "钙", key: "Calcium", minWidth: 80, align: "center" },
        {
          title: "无机磷",
          key: "InorganicPhosphorus",
          minWidth: 80,
          align: "center"
        },
        {
          title: "钙磷乘积",
          key: "CalciumAndPhosphorus",
          minWidth: 80,
          align: "center"
        },
        {
          title: "操作",
          key: "handle",
          minWidth: 80,
          align: "center",
          fixed: "right",
          render: (h, params) => {
            return h("div", [
              h(
                "Tooltip",
                {
                  props: {
                    placement: "top",
                    content: "修改",
                    transfer: true
                  }
                },
                [
                  h("Icon", {
                    props: {
                      type: "md-create"
                    },
                    style: {
                      marginRight: "5px",
                      fontSize: "18px",
                      color: "#2D8CF0",
                      cursor: "pointer"
                    },
                    on: {
                      click: () => {
                        this.updateItem(params.row);
                      }
                    }
                  })
                ]
              ),
              h(
                "Tooltip",
                {
                  props: {
                    placement: "top",
                    content: "删除",
                    transfer: true
                  }
                },
                [
                  h("Icon", {
                    props: { type: "md-close" },
                    style: {
                      fontSize: "18px",
                      color: "red",
                      cursor: "pointer"
                    },
                    on: {
                      click: () => {
                        this.deleteItem(params.row.Id);
                      }
                    }
                  })
                ]
              )
            ]);
          }
        }
      ],
      // 肾功能
      RenalColumns: [
        { title: "送检日期", key: "CheckDate", minWidth: 110, align: "center" },
        {
          title: "检验机构",
          key: "Institutions",
          minWidth: 80,
          align: "center"
        },
        {
          title: "血清肌酐（透前）",
          key: "SerumCreatinineBeforeDialysis",
          minWidth: 80,
          align: "center"
        },
        {
          title: "血清肌酐（透后）",
          key: "SerumCreatinineAfterDialysis",
          minWidth: 80,
          align: "center"
        },
        {
          title: "血清尿酸（透前）",
          key: "SerumUricAcidBeforeDialysis",
          minWidth: 80,
          align: "center"
        },
        {
          title: "血清尿酸（透后）",
          key: "SerumUricAcidAfterDialysis",
          minWidth: 80,
          align: "center"
        },
        {
          title: "尿素（透前）",
          key: "UreaBeforeDialysis",
          minWidth: 80,
          align: "center"
        },
        {
          title: "尿素（透后）",
          key: "UreaAfterDialysis",
          minWidth: 80,
          align: "center"
        },
        {
          title: "血清β2微球蛋白（透前）",
          key: "SerumMicroglobulinBeforeDialysis",
          minWidth: 80,
          align: "center"
        },
        {
          title: "血清β2微球蛋白（透后）",
          key: "SerumMicroglobulinAfterDialysis",
          minWidth: 80,
          align: "center"
        },
        {
          title: "操作",
          key: "handle",
          minWidth: 80,
          align: "center",
          fixed: "right",
          render: (h, params) => {
            return h("div", [
              h(
                "Tooltip",
                {
                  props: {
                    placement: "top",
                    content: "修改",
                    transfer: true
                  }
                },
                [
                  h("Icon", {
                    props: {
                      type: "md-create"
                    },
                    style: {
                      marginRight: "5px",
                      fontSize: "18px",
                      color: "#2D8CF0",
                      cursor: "pointer"
                    },
                    on: {
                      click: () => {
                        this.updateItem(params.row);
                      }
                    }
                  })
                ]
              ),
              h(
                "Tooltip",
                {
                  props: {
                    placement: "top",
                    content: "删除",
                    transfer: true
                  }
                },
                [
                  h("Icon", {
                    props: { type: "md-close" },
                    style: {
                      fontSize: "18px",
                      color: "red",
                      cursor: "pointer"
                    },
                    on: {
                      click: () => {
                        this.deleteItem(params.row.Id);
                      }
                    }
                  })
                ]
              )
            ]);
          }
        }
      ],
      // 肝功、血脂、血糖
      LiverColumns: [
        { title: "送检日期", key: "CheckDate", minWidth: 110, align: "center" },
        {
          title: "血清总蛋白",
          key: "def9da4RcCHbY3875608394492678144",
          minWidth: 80,
          align: "center"
        },
        {
          title: "血清总胆红素",
          key: "def9da4lmma9e3875608427409575937",
          minWidth: 80,
          align: "center"
        },
        {
          title: "乳酸脱氢酶",
          key: "def9da4kvu0eN3875608456782286849",
          minWidth: 80,
          align: "center"
        },
        {
          title: "血清间接胆红素",
          key: "def9da493Xnde3875608484284338176",
          minWidth: 80,
          align: "center"
        },
        {
          title: "血清球蛋白",
          key: "def9da4Cymau43875608513602523137",
          minWidth: 80,
          align: "center"
        },
        {
          title: "血清γ-谷氨酰基转移酶",
          key: "def9da4TjMeG53875608540588675073",
          minWidth: 80,
          align: "center"
        },
        {
          title: "血清直接胆红素",
          key: "def9da4SqxuxN3875608568229138433",
          minWidth: 80,
          align: "center"
        },
        { title: "AST/ALT", key: "def9da4AexfwU3875608598734311425", minWidth: 80, align: "center" },
        {
          title: "血清天门冬氨酸氨基转移酶",
          key: "def9da4xsIytl3875608632016113664",
          minWidth: 80,
          align: "center"
        },
        {
          title: "血清丙氨酸氨基转移酶",
          key: "def9da4ZU5Axu3875608657194520577",
          minWidth: 80,
          align: "center"
        },
        {
          title: "血清碱性磷酸酶",
          key: "def9da4Xcwikg3875608682658140161",
          minWidth: 80,
          align: "center"
        },
        {
          title: "血清白蛋白",
          key: "def9da4pMP7JP3875608715814113280",
          minWidth: 80,
          align: "center"
        },
        {
          title: "血清前白蛋白",
          key: "def9da4d2PaGK3875608743190335489",
          minWidth: 80,
          align: "center"
        },
        {
          title: "总胆固醇",
          key: "def9da4dQaZPN3875608771438972929",
          minWidth: 80,
          align: "center"
        },
        {
          title: "甘油三酯",
          key: "def9da4ItLr8q3875608798618062849",
          minWidth: 80,
          align: "center"
        },
        {
          title: "高密度脂蛋白",
          key: "def9da4vUOpM83875608833376260096",
          minWidth: 80,
          align: "center"
        },
        {
          title: "低密度脂蛋白",
          key: "def9da4IgfCIf3875608859422887937",
          minWidth: 80,
          align: "center"
        },
        { title: "血糖", key: "def9da45TeH7b3875608884815204353", minWidth: 80, align: "center" },
        // {
        //   title: "操作",
        //   key: "handle",
        //   minWidth: 80,
        //   align: "center",
        //   fixed: "right",
        //   render: (h, params) => {
        //     return h("div", [
        //       h(
        //         "Tooltip",
        //         {
        //           props: {
        //             placement: "top",
        //             content: "修改",
        //             transfer: true
        //           }
        //         },
        //         [
        //           h("Icon", {
        //             props: {
        //               type: "md-create"
        //             },
        //             style: {
        //               marginRight: "5px",
        //               fontSize: "18px",
        //               color: "#2D8CF0",
        //               cursor: "pointer"
        //             },
        //             on: {
        //               click: () => {
        //                 this.updateItem(params.row);
        //               }
        //             }
        //           })
        //         ]
        //       ),
        //       h(
        //         "Tooltip",
        //         {
        //           props: {
        //             placement: "top",
        //             content: "删除",
        //             transfer: true
        //           }
        //         },
        //         [
        //           h("Icon", {
        //             props: { type: "md-close" },
        //             style: {
        //               fontSize: "18px",
        //               color: "red",
        //               cursor: "pointer"
        //             },
        //             on: {
        //               click: () => {
        //                 this.deleteItem(params.row.Id);
        //               }
        //             }
        //           })
        //         ]
        //       )
        //     ]);
        //   }
        // }
      ],
      // 传染病监测
      InfectiousColumns: [
        { title: "送检日期", key: "CheckDate", minWidth: 110, align: "center" },
        {
          title: "检验机构",
          key: "Institutions",
          minWidth: 80,
          align: "center"
        },
        {
          title: "乙肝五项",
          key: "TypeId",
          align: "center",
          children: [
            { title: "HBsAg", key: "HBsAg", minWidth: 65 },
            { title: "HBsAb", key: "HBsAb", minWidth: 65 },
            { title: "HBeAg", key: "HBeAg", minWidth: 65 },
            { title: "HBeAb", key: "HBeAb", minWidth: 65 },
            { title: "HBcAb", key: "HBcAb", minWidth: 65 }
          ]
        },
        {
          title: "丙肝抗体",
          key: "HepatitisCAntibody",
          minWidth: 80,
          align: "center"
        },
        {
          title: "人免疫缺陷病毒抗体",
          key: "HIVAntibody",
          minWidth: 80,
          align: "center"
        },
        {
          title: "梅毒",
          key: "TypeId",
          minWidth: 80,
          align: "center",
          children: [
            { title: "梅毒抗体", key: "SyphilisAntibody", minWidth: 75 },
            { title: "TRUST", key: "TRUST", minWidth: 65 }
          ]
        },
        { title: "登记人", key: "Registrar", minWidth: 80, align: "center" },
        { title: "备注", key: "Remark", minWidth: 80, align: "center" },
        {
          title: "操作",
          key: "handle",
          minWidth: 80,
          align: "center",
          fixed: "right",
          render: (h, params) => {
            return h("div", [
              h(
                "Tooltip",
                {
                  props: {
                    placement: "top",
                    content: "修改",
                    transfer: true
                  }
                },
                [
                  h("Icon", {
                    props: {
                      type: "md-create"
                    },
                    style: {
                      marginRight: "5px",
                      fontSize: "18px",
                      color: "#2D8CF0",
                      cursor: "pointer"
                    },
                    on: {
                      click: () => {
                        this.updateItem(params.row);
                      }
                    }
                  })
                ]
              ),
              h(
                "Tooltip",
                {
                  props: {
                    placement: "top",
                    content: "删除",
                    transfer: true
                  }
                },
                [
                  h("Icon", {
                    props: { type: "md-close" },
                    style: {
                      fontSize: "18px",
                      color: "red",
                      cursor: "pointer"
                    },
                    on: {
                      click: () => {
                        this.deleteItem(params.row.Id);
                      }
                    }
                  })
                ]
              )
            ]);
          }
        }
      ],
      // 铁参数测定
      IronColumns: [
        { title: "送检日期", key: "CheckDate", minWidth: 110, align: "center" },
        {
          title: "检验机构",
          key: "Institutions",
          minWidth: 80,
          align: "center"
        },
        {
          title: "血清铁离子",
          key: "SerumIronIon",
          minWidth: 80,
          align: "center"
        },
        { title: "铁蛋白", key: "Ferritin", minWidth: 80, align: "center" },
        {
          title: "总铁结合力",
          key: "TotalIronBindingForce",
          minWidth: 80,
          align: "center"
        },
        {
          title: "转铁蛋白",
          key: "Transferrin",
          minWidth: 80,
          align: "center"
        },
        {
          title: "转铁蛋白饱和度",
          key: "TransferrinSaturation",
          minWidth: 80,
          align: "center"
        },
        {
          title: "操作",
          key: "handle",
          minWidth: 80,
          align: "center",
          fixed: "right",
          render: (h, params) => {
            return h("div", [
              h(
                "Tooltip",
                {
                  props: {
                    placement: "top",
                    content: "修改",
                    transfer: true
                  }
                },
                [
                  h("Icon", {
                    props: {
                      type: "md-create"
                    },
                    style: {
                      marginRight: "5px",
                      fontSize: "18px",
                      color: "#2D8CF0",
                      cursor: "pointer"
                    },
                    on: {
                      click: () => {
                        this.updateItem(params.row);
                      }
                    }
                  })
                ]
              ),
              h(
                "Tooltip",
                {
                  props: {
                    placement: "top",
                    content: "删除",
                    transfer: true
                  }
                },
                [
                  h("Icon", {
                    props: { type: "md-close" },
                    style: {
                      fontSize: "18px",
                      color: "red",
                      cursor: "pointer"
                    },
                    on: {
                      click: () => {
                        this.deleteItem(params.row.Id);
                      }
                    }
                  })
                ]
              )
            ]);
          }
        }
      ],
      // 心肌酶五项
      MyocardialColumns: [
        { title: "送检日期", key: "CheckDate", minWidth: 110, align: "center" },
        {
          title: "检验机构",
          key: "Institutions",
          minWidth: 80,
          align: "center"
        },
        {
          title: "血清天门冬氨酸氨基转移酶",
          key: "SerumAsparticAcidTransferase",
          minWidth: 80,
          align: "center"
        },
        {
          title: "肌酸激酶",
          key: "CreatineKinase",
          minWidth: 80,
          align: "center"
        },
        {
          title: "乳酸脱氢酶",
          key: "LactateDehydrogenase",
          minWidth: 80,
          align: "center"
        },
        {
          title: "肌酸激酶同工酶",
          key: "CreatineKinaseIsozyme",
          minWidth: 80,
          align: "center"
        },
        {
          title: "a一羟丁酸脱氢酶",
          key: "HydroxybutyrateDehydrogenase",
          minWidth: 80,
          align: "center"
        },
        {
          title: "操作",
          key: "handle",
          minWidth: 80,
          align: "center",
          fixed: "right",
          render: (h, params) => {
            return h("div", [
              h(
                "Tooltip",
                {
                  props: {
                    placement: "top",
                    content: "修改",
                    transfer: true
                  }
                },
                [
                  h("Icon", {
                    props: {
                      type: "md-create"
                    },
                    style: {
                      marginRight: "5px",
                      fontSize: "18px",
                      color: "#2D8CF0",
                      cursor: "pointer"
                    },
                    on: {
                      click: () => {
                        this.updateItem(params.row);
                      }
                    }
                  })
                ]
              ),
              h(
                "Tooltip",
                {
                  props: {
                    placement: "top",
                    content: "删除",
                    transfer: true
                  }
                },
                [
                  h("Icon", {
                    props: { type: "md-close" },
                    style: {
                      fontSize: "18px",
                      color: "red",
                      cursor: "pointer"
                    },
                    on: {
                      click: () => {
                        this.deleteItem(params.row.Id);
                      }
                    }
                  })
                ]
              )
            ]);
          }
        }
      ],
      // 高血压四项
      HighBloodPresureColumns: [
        { title: "送检日期", key: "CheckDate", minWidth: 110, align: "center" },
        {
          title: "检验机构",
          key: "Institutions",
          minWidth: 80,
          align: "center"
        },
        {
          title: "肾素活性",
          key: "ReninActivity",
          minWidth: 80,
          align: "center"
        },
        {
          title: "血管紧张素I",
          key: "Angiotensin_I",
          minWidth: 80,
          align: "center"
        },
        {
          title: "血管紧张素II",
          key: "Angiotensin_II",
          minWidth: 80,
          align: "center"
        },
        { title: "醛固酮", key: "Aldosterone", minWidth: 80, align: "center" },
        {
          title: "操作",
          key: "handle",
          minWidth: 80,
          align: "center",
          fixed: "right",
          render: (h, params) => {
            return h("div", [
              h(
                "Tooltip",
                {
                  props: {
                    placement: "top",
                    content: "修改",
                    transfer: true
                  }
                },
                [
                  h("Icon", {
                    props: {
                      type: "md-create"
                    },
                    style: {
                      marginRight: "5px",
                      fontSize: "18px",
                      color: "#2D8CF0",
                      cursor: "pointer"
                    },
                    on: {
                      click: () => {
                        this.updateItem(params.row);
                      }
                    }
                  })
                ]
              ),
              h(
                "Tooltip",
                {
                  props: {
                    placement: "top",
                    content: "删除",
                    transfer: true
                  }
                },
                [
                  h("Icon", {
                    props: { type: "md-close" },
                    style: {
                      fontSize: "18px",
                      color: "red",
                      cursor: "pointer"
                    },
                    on: {
                      click: () => {
                        this.deleteItem(params.row.Id);
                      }
                    }
                  })
                ]
              )
            ]);
          }
        }
      ],
      // 贫血三项
      AnemiaColumns: [
        { title: "送检日期", key: "CheckDate", minWidth: 110, align: "center" },
        {
          title: "检验机构",
          key: "Institutions",
          minWidth: 80,
          align: "center"
        },
        { title: "铁蛋白", key: "Ferritin", minWidth: 80, align: "center" },
        { title: "叶酸", key: "FolicAcid", minWidth: 80, align: "center" },
        {
          title: "维生素B12",
          key: "Vitamin_B12",
          minWidth: 80,
          align: "center"
        },
        {
          title: "操作",
          key: "handle",
          minWidth: 80,
          align: "center",
          fixed: "right",
          render: (h, params) => {
            return h("div", [
              h(
                "Tooltip",
                {
                  props: {
                    placement: "top",
                    content: "修改",
                    transfer: true
                  }
                },
                [
                  h("Icon", {
                    props: {
                      type: "md-create"
                    },
                    style: {
                      marginRight: "5px",
                      fontSize: "18px",
                      color: "#2D8CF0",
                      cursor: "pointer"
                    },
                    on: {
                      click: () => {
                        this.updateItem(params.row);
                      }
                    }
                  })
                ]
              ),
              h(
                "Tooltip",
                {
                  props: {
                    placement: "top",
                    content: "删除",
                    transfer: true
                  }
                },
                [
                  h("Icon", {
                    props: { type: "md-close" },
                    style: {
                      fontSize: "18px",
                      color: "red",
                      cursor: "pointer"
                    },
                    on: {
                      click: () => {
                        this.deleteItem(params.row.Id);
                      }
                    }
                  })
                ]
              )
            ]);
          }
        }
      ],
      // 凝血五项
      CoagulationColumns: [
        { title: "送检日期", key: "CheckDate", minWidth: 110, align: "center" },
        {
          title: "检验机构",
          key: "Institutions",
          minWidth: 80,
          align: "center"
        },
        {
          title: "血浆凝血酶原时间",
          key: "PlasmaProthrombinTime",
          minWidth: 80,
          align: "center"
        },
        {
          title: "血浆凝血酶原活动度",
          key: "PlasmaProthrombinActivity",
          minWidth: 80,
          align: "center"
        },
        {
          title: "活化部分凝血酶原时间",
          key: "PartialThrombinActivationTime",
          minWidth: 80,
          align: "center"
        },
        {
          title: "凝血酶时间",
          key: "ThrombinTime",
          minWidth: 80,
          align: "center"
        },
        {
          title: "纤维蛋白原",
          key: "Fibrinogen",
          minWidth: 80,
          align: "center"
        },
        { title: "D二聚体", key: "D_Dimer", minWidth: 80, align: "center" },
        {
          title: "操作",
          key: "handle",
          minWidth: 80,
          align: "center",
          fixed: "right",
          render: (h, params) => {
            return h("div", [
              h(
                "Tooltip",
                {
                  props: {
                    placement: "top",
                    content: "修改",
                    transfer: true
                  }
                },
                [
                  h("Icon", {
                    props: {
                      type: "md-create"
                    },
                    style: {
                      marginRight: "5px",
                      fontSize: "18px",
                      color: "#2D8CF0",
                      cursor: "pointer"
                    },
                    on: {
                      click: () => {
                        this.updateItem(params.row);
                      }
                    }
                  })
                ]
              ),
              h(
                "Tooltip",
                {
                  props: {
                    placement: "top",
                    content: "删除",
                    transfer: true
                  }
                },
                [
                  h("Icon", {
                    props: { type: "md-close" },
                    style: {
                      fontSize: "18px",
                      color: "red",
                      cursor: "pointer"
                    },
                    on: {
                      click: () => {
                        this.deleteItem(params.row.Id);
                      }
                    }
                  })
                ]
              )
            ]);
          }
        }
      ],
      // 其他
      OtherColumns: [
        { title: "送检日期", key: "CheckDate", minWidth: 110, align: "center" },
        {
          title: "检验机构",
          key: "Institutions",
          minWidth: 80,
          align: "center"
        },
        {
          title: "糖化血红蛋白",
          key: "GlycosylatedHemoglobin",
          minWidth: 80,
          align: "center"
        },
        {
          title: "超敏C反应蛋白",
          key: "ReactiveProtein",
          minWidth: 80,
          align: "center"
        },
        {
          title: "抗链球菌溶血素O",
          key: "AntistreptococcalHemolysin_O",
          minWidth: 80,
          align: "center"
        },
        {
          title: "类风湿因子",
          key: "RheumatoidFactor",
          minWidth: 80,
          align: "center"
        },
        {
          title: "降钙素原",
          key: "CalcitoninOriginal",
          minWidth: 80,
          align: "center"
        },
        {
          title: "同型半胱氨酸",
          key: "Homocysteine",
          minWidth: 80,
          align: "center"
        },
        {
          title: "N端-B型利钠肽前体",
          key: "NatriureticPeptidePrecursor",
          minWidth: 80,
          align: "center"
        },
        { title: "甲胎蛋白", key: "AFP", minWidth: 80, align: "center" },
        {
          title: "癌胚抗原",
          key: "CarcinoembryonicAntigen",
          minWidth: 80,
          align: "center"
        },
        {
          title: "操作",
          key: "handle",
          minWidth: 80,
          align: "center",
          fixed: "right",
          render: (h, params) => {
            return h("div", [
              h(
                "Tooltip",
                {
                  props: {
                    placement: "top",
                    content: "修改",
                    transfer: true
                  }
                },
                [
                  h("Icon", {
                    props: {
                      type: "md-create"
                    },
                    style: {
                      marginRight: "5px",
                      fontSize: "18px",
                      color: "#2D8CF0",
                      cursor: "pointer"
                    },
                    on: {
                      click: () => {
                        this.updateItem(params.row);
                      }
                    }
                  })
                ]
              ),
              h(
                "Tooltip",
                {
                  props: {
                    placement: "top",
                    content: "删除",
                    transfer: true
                  }
                },
                [
                  h("Icon", {
                    props: { type: "md-close" },
                    style: {
                      fontSize: "18px",
                      color: "red",
                      cursor: "pointer"
                    },
                    on: {
                      click: () => {
                        this.deleteItem(params.row.Id);
                      }
                    }
                  })
                ]
              )
            ]);
          }
        }
      ], */
  
      }
    },
    computed: {
      ducumentApi () {
        // 解决缓存问题
        let baseUrl = process.env.NODE_ENV === 'development'
          ? this.$config.baseURL.dev
          : this.$config.baseURL.pro
        return `${baseUrl}Document/Patientsfileview/${this.medicalRecordChecked.id}/${this.medicalType}?date=${new Date().getTime()}`
      }
    },
    created(){
    
    // this.GetCategoryLook()
    // this.changeTab('def9da41kS5HI3874150768152023040',1,'电解质七项')
    },
    mounted () {
      this.$nextTick(() => {
        this.changeTab()
        this.GetCategoryList();

      })
    },
    methods: {

        /* 
            new
        */

        //获取所有检验项目分类配置
        GetCategoryList(){
            this.swsApi.swsPost('Patients/GetLaboratoryCategorySetting').then(res=>{
                console.log(res);
                if(res.status == 200){
                    this.tabData = res.data.result.map((item,index)=>{
                        return {id:item.id,name:item.displayName,index:index+1}
                    })
                    console.log(this.tabData);

                    setTimeout(() => {
                        this.changeTab(this.tabData[0].id,this.tabData[0].index,this.tabData[0].name)
                    }, 100);
                }
            })
        },
        //根据检验项目查询  Patients/GetLaboratoryCategorySetting
        GetCategoryLook(id){
            this.swsApi.swsGet(`Patients/GetLaboratoryCategorySetting/${id}`).then(res=>{
                if(res.data.success){
                    console.log(res);
                    this.table_columns = res.data.result.map(item=>{
                        return {title:item.displayName,key:item.id,minWidth:80,align:'center'}
                    })

                    this.table_columns.unshift( { title: "送检日期", key: "CheckDate", minWidth: 110, align: "center",fixed:"left" })
                    // this.table_columns.push( { title: "操作", key: "handle", minWidth: 110, align: "center",fixed: "right"})
                    console.log(this.table_columns);
                }
                
            })
        },
        exportTable() {
      this.$refs.table.exportCsv({
        filename: this.medicalCheckedInfo.hospitalName+this.tabCheckedInfo.name+'-'+this.medicalCheckedInfo.patientName,
        columns: this.table_columns.filter((col, index) => {
          return index < this.table_columns.length-1;
        }),
        data: this.table_data.filter((data, index) => {
          if(data.CheckDate){
			  data.CheckDate = data.CheckDate.substring(0,10);
		  }
          return index < this.table_data.length;
        })
      });
    },


        /* 
            new
        */


      // cy 下载文档
      downLoadPDF () {
        let src = this.$refs['pdfADom'].getAttribute('href')
        // 下载文件
        this.swsApi.swsDownload({
          methods: 'get',
          url: `${src}?date=${new Date().valueOf()}`,
          responseType: 'blob'
        }).then(res => {
          let blob = new Blob([res.data], {type: 'application/pdf'})
          let elink = document.createElement('a')
          elink.download = '病历.pdf'
          elink.style.display = 'none'
          let href = URL.createObjectURL(blob)
          elink.href = href
          document.body.appendChild(elink)
          elink.click()
          URL.revokeObjectURL(href)
          document.body.removeChild(elink)
        })
      },
      // cy 隐藏详情
      hide () {
        this.table_detail_flag = false
      },
      // cy 查看表单详情
      showDetail (row) {
        this.table_detail_data = []
        this.table_detail_flag = true
        this.formDetail = row
        let params = {}
        let detailUrl = ''
        if (this.tabCheckedInfo.id === 2) {
          params = { balanceNo: row.balanceNo }
          detailUrl = 'MedicalRcord/MedicalRcord/FeesforListingDetail'
          this.table_detail_columns = this.table_charge_detail_columns
        } else if (this.tabCheckedInfo.id === 3) {
          params = { prescriptionId: row.id }
          detailUrl = 'MedicalRcord/MedicalRcord/PrescriptionDetail'
          this.table_detail_columns = this.table_prescription_detail_columns
        }
        params.centerId = this.medicalCheckedInfo.hospitalId
        this.tableDetailLoading = true
        this.swsApi
          .swsPost(detailUrl, params)
          .then(res => {
            if (res.data.success) {
              this.tableDetailLoading = false
              this.table_detail_data = res.data.result
              this.prescriptionTotalMoney = this.countMoney(res.data.result)
              this.$nextTick(res => {
                // cy 调整table高度
                this.tableHeight = document.documentElement.clientHeight - 365
              })
            }
          })
          .catch(e => {
            this.$Notice.error({
              title: '请求错误,请稍后再试',
              desc: e
            })
            this.tableDetailLoading = false
          })
      },
      // cy 计算处方单明细的总金额
      countMoney (data) {
        let money = 0
        data.forEach(item => {
          // cy 只有当状态为0且退费金额不为0时 才有必要让应收金额减去退费金额
          if (item.discountStatus === 0 && item.refundPrice) {
            money = accAdd(
              money,
              subtract(item.receivablePrice, item.refundPrice)
            )
          } else {
            money = accAdd(money, item.receivablePrice)
          }
        })
        return parseFloat(money.toFixed(3))
      },
      changeDate (arr) {
        if(!arr[0] && !arr[1]){
            this.beginTime = '';
            this.endTime = '';
            this.getTableData()
        }else{
            this.beginTime = new Date(arr[0]).toISOString()
            this.endTime = new Date(arr[1]).toISOString()
            this.getTableData()
        }
      },
    //   clearDate(){
    //     this.beginTime = '';
    //     this.endTime = '';
    //     this.getTableData()
    //   },
      // cy 切换标签
      changeTab (id,index,name) {
        console.log(id);
        console.log(name);
        id = id || 'def9da41kS5HI3874150768152023040'
        this.tabCheckedInfo = {
          id: id,
          name: name
        }
        this.hide()
        this.pageIndex = 1
        this.GetCategoryLook(id)

        this.getTableData()
      },
      changePatient (info) {
        console.log(info);

        this.medicalCheckedInfo = info

        this.hide()
        this.pageIndex = 1
        this.getTableData()
      },
      getTableData () {
        let params = {
          // centerId: 'abc3d60b474c4fef946a66887348c41a',
          // patientId: '94dc006a4cd54e78b152c500f99d81b4',
          centerId: this.medicalCheckedInfo.hospitalId,
          patientId: this.medicalCheckedInfo.patientId,
          categoryId:this.tabCheckedInfo.id,
          startTime: this.beginTime,
          endTime: this.endTime,
          pageSize: this.pageSize,
          pageIndex: this.pageIndex
        }
      


      /*   //测试
        switch (index) {
          case 1://电解质七项
            // this.table_columns =this.ElectrolyteColumns
            this.GetCategoryLook(this.tabCheckedInfo.id)
            break
          case 2://肝功、血脂、血糖
            // this.table_columns = this.LiverColumns
            this.GetCategoryLook(this.tabCheckedInfo.id)

            break
          case 3://血常规
            // this.table_columns = this.BloodColumns
            this.GetCategoryLook(this.tabCheckedInfo.id)

            break
          case 4://"凝血五项"
            // this.table_columns = this.CoagulationColumns
            this.GetCategoryLook(this.tabCheckedInfo.id)

            break
          case 5://"肾功能"
            // this.table_columns = this.RenalColumns
        this.GetCategoryLook(this.tabCheckedInfo.id)

            break
          case 6://其他
            // this.table_columns = this.OtherColumns
        this.GetCategoryLook(this.tabCheckedInfo.id)

            break
          case 7://"心肌酶五项"
            // this.table_columns = this.MyocardialColumns
        this.GetCategoryLook(this.tabCheckedInfo.id)

            break
          case 8://"传染病检测"  待确定
            // this.table_columns = this.InfectiousColumns
        this.GetCategoryLook(this.tabCheckedInfo.id)

            break
          case 9://"铁参数测定"
            // this.table_columns = this.IronColumns
        this.GetCategoryLook(this.tabCheckedInfo.id)

            break
          case 10://"贫血三项"
            // this.table_columns = this.AnemiaColumns
        this.GetCategoryLook(this.tabCheckedInfo.id)

            break
          case 11://"高血压四项"
            // this.table_columns = this.HighBloodPresureColumns
        this.GetCategoryLook(this.tabCheckedInfo.id)

            break
          case 12://"矿物质及骨代谢"
            // this.table_columns = this.MineralsColumns
        this.GetCategoryLook(this.tabCheckedInfo.id)

            break
        } */
        this.loading = true

        if(this.medicalCheckedInfo.patientId){
            console.log(params);
            this.swsApi
            .swsPost('Patients/GetInspResultRecords', params)
            .then(res => {
              if (res.data.success) {
                console.log(res);
                this.table_data = res.data.result
                this.dataCount = res.data.dataCount

                console.log(this.table_data);
              }
              this.loading = false
            })
            .catch(e => {
              this.$Notice.error({
                title: '请求错误,请稍后再试',
                desc: e
              })
              this.loading = false
            })
        }
      },
      handleChangePage (i) {
        this.pageIndex = i
        this.getTableData()
      }
    }
  }
  </script>
  
  <style scoped lang="less">
  #medical {
    position: relative;
    height: 100%;
    /deep/ .content {
      // margin-right: 20px;
      width: 100%;
      // background: #ffffff;
      overflow-y: auto;
    }
    .tab-panel {
      position: sticky;
      top: 0;
      z-index: 4;
      &-item {
        position: relative;
        display: inline-block;
        font-size: 11px;
        // width: 90px;
        height: 32px;
        line-height: 32px;
        text-align: center;
        background-color: #ffffff;
        border: solid 1px #eaeaea;
  
        border-left: none;
        cursor: pointer;
        &:first-child {
          border-left: solid 1px #eaeaea;
          &.active {
            border-left-color: transparent;
          }
        }
        &.active {
          color: #ee5151;
          border-bottom: none;
          border-top: solid 1px #ff6e5c;
        }
        &:hover {
          color: #ee5151;
        }
      }
    }
    .table {
      background: #ffffff;
      overflow-y: auto;
      padding: 20px 20px 40px;
      .right-group {
        float: right;
        cursor: pointer;
        line-height: 37px;
        font-size: 14px;
        margin-right: 30px;
        color: #5a9be9;
      }
      .top-btn-group {
        span {
          margin-right: 20px;
          width: 60px;
          text-align: right;
        }
      }
      .table-content {
        // margin-top: 20px;
        .pdf-box {
          
          h4 {
            display: inline-block;
            padding-left: 10px;
            border-left: 3px solid #3399ff;
            margin: 15px 10px 0 0;
            font-size: 13px;
            line-height: 13px;
          }
          .pdf-list {
            float: right;
            color: #333333;
            width: 183px;
            font-size: 14px;
            text-align: center;
            .pdf-list-box {
              margin-top: 20px;
              padding: 10px 15px;
              border-radius: 4px;
              border: solid 1px #eaeaea;
              font-size: 13px;
              text-align: left;
              .pdf-list-text {
                color: #555555;
                // font-weight: 600;
                border-bottom: 1px dashed #ddd;
                line-height: 33px;
                &:hover {
                  background: #f0f0f0;
                  cursor: pointer;
                }
              }
              .active {
                background: #f0f0f0;
              }
            }
            &.disable + * {
              .default-table {
                margin-left: 0 !important;
              }
            }
          }
        }
      }
    }
    .table2 {
      position: relative;
      padding: 20px 10px 1px;
      background: #ffffff;
      .ivu-table-wrapper {
        & /deep/ .ivu-table .ivu-table-cell {
          padding-right: 8px;
          padding-left: 8px;
        }
      }
      h2 {
        text-align: center;
        font-size: 18px;
        font-weight: 500;
        margin-bottom: 20px;
      }
      .btn-groups {
        color: #999;
        font-size: 13px;
        span {
          margin-right: 20px;
          display: inline-block;
          // width: 60px;
          text-align: right;
        }
        & + .btn-groups {
          margin: 20px 0;
        }
        & > * {
          margin-right: 20px;
        }
        /deep/ .ivu-radio-group-button .ivu-radio-wrapper-checked {
          color: #ffffff;
          background: #4f95e8;
        }
      }
      .table2-content {
        margin-top: 20px;
      }
      .table_detail {
        position: absolute;
        background: #ffffff;
        top: 42px;
        left: 0;
        overflow-y: auto;
        padding: 0 20px;
        z-index: 4;
        width: 100%;
        .head_content {
          .record {
            margin: 20px 0 10px;
            color: #333333;
            font-size: 16px;
            span {
              margin-left: 10px;
            }
            .right {
              float: right;
              .approval {
                background: #f90;
                color: #ffffff;
                border-color: #f90;
              }
            }
            &::after {
              display: block;
              content: '';
              height: 0;
              visibility: hidden;
              clear: both;
            }
          }
          .items-box {
            font-size: 14px;
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
      }
    }
  }
  .color-gray {
    color: #999;
  }
  .empty {
    margin-top: 20px;
    font-size: 14px;
    line-height: 80px;
    text-align: center;
  }
  .status-text-success {
    color: #19be6b;
  }
  .status-text-error {
    color: #ed4014;
  }
  /deep/ .ivu-table {
    .status-text-success {
      .status-text-success;
    }
    .status-text-error {
      .status-text-error;
    }
  }
  </style>
  