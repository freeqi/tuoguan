<template>
  <div class="user-avator-dropdown">
    <Dropdown @on-click="handleClick">
      <span>{{empName}}</span>
      <Icon :size="18" type="md-arrow-dropdown"></Icon>
      <DropdownMenu slot="list">
        <DropdownItem name="changePwd">修改密码</DropdownItem>
        <DropdownItem name="logout">退出登录</DropdownItem>
      </DropdownMenu>
    </Dropdown>
    <Modal style="text-align: center;" width="360px" class-name="vertical-center-modal" :mask-closable="false" v-model="changePwdModal" title="修改登录密码">
      <Form ref="cform" :model="cform" :rules="cruleForm" :label-width="80">
        <FormItem  label="原密码" prop="olPwd">
          <Input type="password" v-model="cform.olPwd" placeholder="原密码">
          </Input>
        </FormItem>
        <FormItem label="新密码" prop="newPwd1">
            <Input type="password" v-model="cform.newPwd1" placeholder="新密码">
            </Input>
          </FormItem>
        <FormItem label="确认密码" prop="newPwd2">
          <Input type="password" v-model="cform.newPwd2" placeholder="确认密码">
          </Input>
        </FormItem>
      </Form>
      <div slot="footer" style="text-align:center">
        <Button type="primary" @click="savePsd()">保存</Button>
        <Button type="default" @click="cancel">取消</Button>
      </div>
    </Modal>
  </div>
</template>

<script>
import './user.less'
import { mapActions, mapMutations, mapState } from 'vuex'
import { Safety } from '@/assets/javascript/aes_1.js'
import { setToken } from '@/libs/util'
export default {
  name: 'User',
  props: {
    userAvator: {
      type: String,
      default: ''
    }
  },
  computed: mapState({
    empName: state => state.user.empName
  }),
  data () {
    const pwdCheckValidate = (rule, value, callback) => {
      if (this.cform.newPwd1 !== '' && value === '') {
        callback(new Error('确认密码不能为空'))
      } else if (this.cform.newPwd1 !== value) {
        callback(new Error('两次密码不一致，请重新输入'))
      } else {
        callback()
      }
    }
    return {
      cform: {
        olPwd: '',
        newPwd1: '',
        newPwd2: ''
      },
      cruleForm: {
        olPwd: [
          { required: true, message: '原密码不能为空', trigger: 'blur' }
        ],
        newPwd1: [
          { required: true, message: '新密码不能为空', trigger: 'blur' }
        ],
        newPwd2: [
          { required: true, validator: pwdCheckValidate, trigger: 'blur' }
        ]
      },
      changePwdModal: false,
      olPwd: '',
      newPwd1: '',
      newPwd2: '',
      userName: sessionStorage.getItem('userName')
    }
  },
  methods: {
    ...mapActions([
      'handleLogOut'
    ]),
    ...mapMutations(['setButtonRole']),
    handleClick (name) {
      switch (name) {
        case 'logout':
          this.handleLogOut().then(() => {
            this.$router.push({
              name: 'login'
            })
          })
          break;
        case 'changePwd':
          this.changePwdModal = true
          break;
      }
    },
    cancel () {
      this.changePwdModal = false
      this.$refs.cform.resetFields()
      this.$Message.info('取消修改密码 ')
    },
    savePsd (name) {
      this.$refs.cform.validate((valid) => {
        if (valid) {
          if (this.cform.newPwd1 !== this.cform.newPwd2) {
            this.$Message.error('两次输入密码不正确，请重新输入!')
            this.cform.newPwd1 = ''
            this.cform.newPwd2 = ''
          } else {
            let params = {
              userName: this.userName,
              olPwd: this.cform.olPwd,
              newPwd: this.cform.newPwd2
            }

            for (let key in params) {
              params[key] = Safety.Encrypt(params[key])
            }

            this.swsApi.swsPost('User/user/mpwd', params).then(res => {
              if (res.data.error === null) {
                this.changePwdModal = false
                this.$Message.success('修改密码成功!')
                setToken('')
                this.$router.push({
                  name: 'login'
                })
              } else {
                this.$Message.error('登录密码修改失败!')
              }
            })
          }
        } else {
          this.$Message.error('请完善必填信息')
        }
      })
    }
  }
}
</script>
