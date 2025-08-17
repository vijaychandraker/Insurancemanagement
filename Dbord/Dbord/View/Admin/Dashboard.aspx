<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="Dashboard.aspx.cs" Inherits="Dbord.View.Admin.Dashboard" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <section class="content-header">
    <div class="container-fluid">
        <div class="row mb-2">
            <div class="col-sm-6">
                <h1>Dashboard</h1>
            </div>
            <div class="col-sm-6">
                <ol class="breadcrumb float-sm-right">
                    <li class="breadcrumb-item"><a href="#">Home</a></li>
                    <li class="breadcrumb-item active">Dashboard</li>
                </ol>
            </div>
        </div>
    </div><!-- /.container-fluid -->
</section>
<section class="content">
    <div class="container-fluid">
        <div class="row">
            <div class="col-lg-4 sm-12">
                <!-- small card -->
                <div class="small-box bg-info">
                    <div class="inner">
                        <h3>Total Active Policy</h3>
                    </div>
                    <div class="icon">
                        <i class="far fa-envelope"></i>
                    </div>
                    <a href="#" class="small-box-footer">
                        More info <i class="fas fa-arrow-circle-right"></i>
                    </a>
                </div>
            </div>
            <!-- ./col -->
            <div class="col-lg-4 sm-12">
                <!-- small card -->
                <div class="small-box bg-success">
                    <div class="inner">
                        <h3>Total<sup style="font-size: 20px"></sup></h3>

                        <p>कुल पूर्ण पत्र</p>
                    </div>
                    <div class="icon">
                        <i class="far fa-envelope"></i>
                    </div>
                    <a href="#" class="small-box-footer">
                        More info <i class="fas fa-arrow-circle-right"></i>
                    </a>
                </div>
            </div>
            <!-- ./col -->
            <div class="col-lg-4 sm-12">
                <!-- small card -->
                <div class="small-box bg-danger">
                    <div class="inner">
                        <h3>Totla</h3>

                        <p>कुल अपूर्ण पत्र</p>
                    </div>
                    <div class="icon">
                        <i class="far fa-envelope"></i>
                    </div>
                    <a href="#" class="small-box-footer">
                        More info <i class="fas fa-arrow-circle-right"></i>
                    </a>
                </div>
            </div>
         
        </div>
    </div>
</section>
<section class="content">
    <div class="container-fluid">
        <div class="row">
            <div class="col-lg-4 sm-12">
                <div class="card card-danger">
                    <div class="card-header">
                        <h3 class="card-title">पूर्ण/अपूर्ण</h3>
                        
                    </div>
                    <div class="card-body">
                        <canvas id="donutChart" style="min-height: 250px; height: 250px; max-height: 250px; max-width: 100%;"></canvas>
                    </div>
                </div>
                </div>
                <div class="col-lg-8 sm-12">
                    <div class="card card-warning">
                        <div class="card-header">
                            <h3 class="card-title">विभागवार ग्राफ</h3>

                           
                        </div>
                        <div class="card-body">
                            <div class="chart">
                                <canvas id="barChart" style="min-height: 250px; height: 250px; max-height: 250px; max-width: 100%;"></canvas>
                            </div>
                        </div>
                        <!-- /.card-body -->
                    </div>
                </div>

            </div>
</div>
</section>
    
</asp:Content>
