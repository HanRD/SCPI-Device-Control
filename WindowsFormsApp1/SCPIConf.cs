using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp1
{
    class SCPIConf
    {
        Ivi.Visa.Interop.ResourceManagerClass rm;
        Ivi.Visa.Interop.FormattedIO488Class ioobj;
        private string _selecteddeviceaddress { get; set; }
        private List<string> selectabledevicelist = new List<string>();
        //控制设备指令入口
        private void controlInstrument(string devicetype, string parameters, string operationtype)
        {
            switch (operationtype)
            {
                case "INIT":
                    {
                        initInstrument(devicetype, parameters);
                        break;
                    }
                case "CONF":
                    {
                        configureParameters(devicetype, parameters);
                        break;
                    }
                case "MEAS_READ":
                    {
                        PerformReadingMeasurement(devicetype, parameters);
                        break;
                    }
                case "MEAS_WAVF":
                    {
                        PerformWaveformMeasurement(devicetype, parameters);
                        break;
                    }
                default:
                    break;
            }
        }

        //初始化设备
        private void initInstrument(string devicetype, string parameters)
        {
            switch (devicetype)
            {
                case "AgilentE4418B":
                    {

                        break;
                    }
                default:
                    {
                        MessageBox.Show("暂不支持这类设备的程序控制", "不支持的设备");
                        break;
                    }
            }
        }

        //功率计置0,如有需要可以在parameter里配置需要置0的信道
        private void zeroPowerMeter(string devicetype,string parameters,string device)
        {
            switch (devicetype)
            {
                case "AgilentE4418B":
                    {
                        string commandstr = "CAL:ZERO:AUTO ONCE";//E4418B功率计置0语句
                        SendSCPIcommand(commandstr,device);
                        break;
                    }
                default:
                    {
                        MessageBox.Show("暂不支持这类设备的程序控制", "不支持的设备");
                        break;
                    }
            }
        }

        //功率计校准,如有需要可以在parameter里配置需要校准的信道
        private void CalibratePowerMeter(string devicetype, string parameters, string device) {
            switch (devicetype)
            {
                case "AgilentE4418B":
                    {
                        string commandstr = "CAL:AUTO ONCE";//E4418B功率计校准语句
                        SendSCPIcommand(commandstr, device);
                        break;
                    }
                default:
                    {
                        MessageBox.Show("暂不支持这类设备的程序控制", "不支持的设备");
                        break;
                    }
            }
        }

        //配置测试参数
        private void configureParameters(string devicetype, string parameters)
        {
            switch (devicetype)
            {
                case "AgilentE4440A":
                    {

                        break;
                    }

                case "AgilentE4418B":
                    {

                        break;
                    }
                default:
                    {
                        MessageBox.Show("暂不支持这类设备的程序控制", "不支持的设备");
                        break;
                    }
            }
        }

        //执行测试
        private void PerformReadingMeasurement(string devicetype, string parameters)
        {
            switch (devicetype)
            {
                case "AgilentE4418B":
                    {

                        break;
                    }
                default:
                    {
                        MessageBox.Show("暂不支持这类设备的程序控制", "不支持的设备");
                        break;
                    }
            }
        }

        //执行波形测试
        private void PerformWaveformMeasurement(string devicetype, string parameters)
        {
            switch (devicetype)
            {
                case "AgilentE4440A":
                    {

                        break;
                    }

                case "AgilentE4418B":
                    {

                        break;
                    }
                default:
                    {
                        MessageBox.Show("暂不支持这类设备的程序控制", "不支持的设备");
                        break;
                    }
            }
        }

        //搜索可用设备
        private List<string> SearchConnectedDevice(string port)
        {
            rm = new Ivi.Visa.Interop.ResourceManagerClass();
            ioobj = new Ivi.Visa.Interop.FormattedIO488Class();
            selectabledevicelist = new List<string>();
            string[] availabledevice;
            string[] deviceInfList;
            try
            {
                availabledevice = (string[])rm.FindRsrc(port + "?*INSTR");
                if (availabledevice.Length > 0)
                {
                    foreach (string addr in availabledevice)
                    {
                        try
                        {
                            ioobj.IO = (Ivi.Visa.Interop.IMessage)rm.Open(addr, Ivi.Visa.Interop.AccessMode.NO_LOCK, 50, "");
                            ioobj.WriteString("*IDN?");
                            deviceInfList = (string[])ioobj.ReadList(Ivi.Visa.Interop.IEEEASCIIType.ASCIIType_Any, ",");
                            string deviceinf = deviceInfList[0] + "," + deviceInfList[1];
                            selectabledevicelist.Add(deviceinf);
                        }
                        catch (Exception)
                        {
                            throw;
                        }
                    }
                }
                else
                { 
                    //备选设备列表置空,提示未搜索到设备

                    MessageBox.Show("没有搜索到可用设备,请确认设备连接状态", "未搜索到设备");

                }
                return selectabledevicelist;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                try
                {
                    Console.WriteLine("Session Ended");
                    ioobj.IO.Close();

                }
                catch { }
                try
                {
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(ioobj);
                }
                catch { }
                try
                {
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(rm);
                }
                catch { }
            }

        }
        public void SendSCPIcommand(string str, string device)
        {
            rm = new Ivi.Visa.Interop.ResourceManagerClass();
            ioobj = new Ivi.Visa.Interop.FormattedIO488Class();
            try
            {
                ioobj.IO = (Ivi.Visa.Interop.IMessage)rm.Open(device, Ivi.Visa.Interop.AccessMode.NO_LOCK, 50, "");//在这里输入设备地址
                ioobj.WriteString(str, true);

            }
            catch (Exception e)
            {
                Console.WriteLine("An error occurred: " + e.Message);
            }
            finally
            {
                try
                {
                    Console.WriteLine("Session Ended");
                    ioobj.IO.Close();

                }
                catch { }
                try
                {
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(ioobj);
                }
                catch { }
                try
                {
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(rm);
                }
                catch { }
            }
        }

        public string SendSCPICommandAndReadString(string str,string device)
        {
            rm = new Ivi.Visa.Interop.ResourceManagerClass();
            ioobj = new Ivi.Visa.Interop.FormattedIO488Class();
            string result;
            try
            {
                ioobj.IO = (Ivi.Visa.Interop.IMessage)rm.Open(device, Ivi.Visa.Interop.AccessMode.NO_LOCK, 50, "");//在这里输入设备地址
                ioobj.WriteString(str, true);
                result = ioobj.ReadString();
                return result;
            }
            catch (Exception e)
            {
                Console.WriteLine("An error occurred: " + e.Message);
                result = "error";
                return result;
            }
            finally
            {
                try
                {
                    Console.WriteLine("Session Ended");
                    ioobj.IO.Close();

                }
                catch { }
                try
                {
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(ioobj);
                }
                catch { }
                try
                {
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(rm);
                }
                catch { }
            }

        }

        private void SendSCPICommandAndReadTrace(string str,string device)
        {
            rm = new Ivi.Visa.Interop.ResourceManagerClass();
            ioobj = new Ivi.Visa.Interop.FormattedIO488Class();
            string result;
            try
            {
                ioobj.IO = (Ivi.Visa.Interop.IMessage)rm.Open(device, Ivi.Visa.Interop.AccessMode.NO_LOCK, 50, "");//在这里输入设备地址
                ioobj.WriteString(str, true);
                result = ioobj.ReadString();
            }
            catch (Exception e)
            {
                Console.WriteLine("An error occurred: " + e.Message);
                result = "error";
            }
            finally
            {
                try
                {
                    Console.WriteLine("Session Ended");
                    ioobj.IO.Close();

                }
                catch { }
                try
                {
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(ioobj);
                }
                catch { }
                try
                {
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(rm);
                }
                catch { }
            }
        }
    }
}

