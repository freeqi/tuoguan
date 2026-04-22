<template>
  <div id="role-permission">
    <div class="header">
      <Row style="margin-left:20px">
        <i-col :sm="24" :md="24" :lg="24">
          <span style="margin-right:12px; font-size:15px; color:#666;">角色类型</span>
          <Select style="width:200px" :label-in-value="true"  @on-change="changeType" placeholder="请选择角色" v-model="roleId">
            <Option v-for="item in roleList" :value="item.id" :key="item.id">{{item.roleName}}</Option>
          </Select>
        </i-col>
      </Row>
      <Row style="margin-left:90px; font-size:15px; margin-top:30px">
        {{roleName}}权限分配
        <Divider />
      </Row>
    </div>
    <div class="content">
      <div  v-if="!loading">
        <div class="tree-box" v-for="item in data" :key="item.id">
          <span class="name">{{item.title}}：</span>
          <div>
            <div class="tree-item" v-for="itemData in item.children" :key="itemData.id">
              <Tree ref="tree" :data-title="item.title" :data="[itemData]" show-checkbox></Tree>
            </div>
          </div>
        </div>
        <div class="btn-group">
          <Button type="primary" v-permission="buttonRole.JSQX_BC" @click="setRole">保存</Button>
          <!-- <Button class="cancelDelete" type="default">取消</Button> -->
        </div>
      </div>
      <Spin v-else fix style="margin-top:100px"></Spin>
    </div>
  </div>
</template>

<script>
const BUTTONROLE = {
  JSQX_BC: 'JSQX_BC'
}
export default {
  name: 'rolePermissions',
  data () {
    return {
      roleList: [],
      roleName: '',
      roleId: '',
      data: [],
      loading: false,

      buttonRole: BUTTONROLE
    }
  },
  mounted () {
    this.$nextTick(() => {
      this.getRoleList()
    })
  },
  methods: {
    getRoleList () {
      this.loading = true
      this.swsApi.swsPost('Role/RoleList')
        .then(res => {
          let data = res.data
          this.roleList = data.result
          this.roleId = this.roleList[0].id
          this.roleName = this.roleList[0].roleName
          this.getMenu(this.roleId)
          this.loading = false
        })
        .catch(e => {
          this.loading = false
        })
    },
    getMenu (id) {
      this.loading = true
      this.swsApi.swsPost(`Permission/get/RolePermissions/${id}`)
        .then(res => {
          this.loading = false
          this.data = res.data.result
        })
        .catch(e => {
          this.loading = false
        })
    },
    changeType (res) {
      this.roleName = res.label
      this.getMenu(this.roleId)
    },
    setRole () {
      let node = []
      this.$refs.tree.map(v => {
        if (v.getCheckedNodes().length === 0) return
        let title = v.$el.getAttribute('data-title')
        let children = v.getCheckedNodes()
        let target = {
          title,
          children
        }
        node.push(target)
      })

      let params = {
        menuButtonCode: node,
        roleld: this.roleId
      }
      // console.log(result)
      this.swsApi.swsPost('Permission/set/RolePermissions', params)
        .then(res => {
          let data = res.data
          if (data.success) {
            this.$Message.success('权限设置成功')
            this.getMenu(this.roleId)
          }
        })
    }
  },
  components: {

  }
}
</script>

<style scoped lang="less">
#role-permission {
  padding-bottom: 20px;
  height:100%;
  background-color:#fff;
  overflow-y: auto;
}
.header {
  padding-top: 20px;
  position: sticky;
  top: 0;
  background: #fff;
  z-index: 100
}
.content { position: relative; }
.tree-box {
  padding-left: 90px;
  display: flex;
  font-size: 15px;
  margin-bottom: 40px;
  .name {
    padding: 9px 20px 0 0;
    width: 200px;
    text-align: right;
  }
  .name + * {
    width: 0;
    flex: 1;
    .tree-item {
      display: inline-block;
      vertical-align: top;
      width: 25%;
      &:nth-child(n+5) {
        margin-top: 10px;
      }
      /deep/ .ivu-tree ul{
        font-size: 15px;
        color: #424242;
      }
    }
  }

}
.btn-group {
  margin-top: 50px;
  text-align: center;
  button + button {
    margin-left: 20px;
  }
}
@media (min-width: 996px) and (max-width: 1200px) {
  .tree-item {
    width: 33.3% !important;
    &:nth-child(n+4) {
      margin-top: 10px;
    }
  }
  .tree-box {
    .name {
      width: 140px !important;
    }
  }
}
@media (min-width: 766px) and (max-width: 995px) {
  .tree-item {
    width: 50% !important;
    &:nth-child(n+3) {
      margin-top: 10px;
    }
  }
  .tree-box {
    .name {
      width: 140px !important;
    }
  }
}
@media (max-width: 765px) {
  .tree-item {
    width: 100% !important;
    &:nth-child(n+2) {
      margin-top: 10px;
    }
  }
  .tree-box {
    .name {
      width: 140px !important;
    }
  }
}
</style>
