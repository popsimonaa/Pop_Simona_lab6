using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data;
using System.Data.Entity;
using AutoLotModel;

namespace Pop_Simona_lab6
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
enum ActionState
    {
        New,
        Edit,
        Delete,
        Nothing
    }
    public partial class MainWindow : Window
    {
        ActionState action = ActionState.Nothing;
        AutoLotEntitiesModel ctx = new AutoLotEntitiesModel();
        CollectionViewSource customerViewSource;
        CollectionViewSource iventoryViewSource;
        CollectionViewSource customerOrdersViewSource;
        Binding txtFirstNameBinding = new Binding();
        Binding txtLastNameBinding = new Binding();
        Binding txtMakeBinding = new Binding();
        Binding txtColorBinding = new Binding();

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            txtFirstNameBinding.Path = new PropertyPath("FirstName");
            firstNAmeTextBox.SetBinding(TextBox.TextProperty, txtFirstNameBinding);
            txtLastNameBinding.Path = new PropertyPath("LastName");
            lastNameTextBox.SetBinding(TextBox.TextProperty, txtLastNameBinding);

            txtMakeBinding.Path = new PropertyPath("Make");
            colorTextBox.SetBinding(TextBox.TextProperty, txtColorBinding);
            txtColorBinding.Path = new PropertyPath("Color");
            makeTextBox.SetBinding(TextBox.TextProperty, txtMakeBinding);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

            System.Windows.Data.CollectionViewSource customerViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("customerViewSource")));
            // Load data by setting the CollectionViewSource.Source property:
            // customerViewSource.Source = [generic data source]
            //using System.Data.Entity;
            customerViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("customerViewSource")));
            customerViewSource.Source = ctx.Customers.Local;
            ctx.Customers.Load(); 
        System.Windows.Data.CollectionViewSource iventoryViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("iventoryViewSource")));
            // Load data by setting the CollectionViewSource.Source property:
            // iventoryViewSource.Source = [generic data source]
            iventoryViewSource.Source = ctx.Iventories.Local;
            customerOrdersViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("customerOrdersViewSource")));
            customerOrdersViewSource.Source = ctx.Orders.Local;
            ctx.Orders.Load();
            ctx.Iventories.Load();
            cmbCustomers.ItemsSource = ctx.Customers.Local;
            cmbCustomers.DisplayMemberPath = "FirstName";
            cmbCustomers.SelectedValuePath = "CustId";
            cmbInventory.ItemsSource = ctx.Iventories.Local;
            //cmbInventory.DisplayMemberPath = "Make";
            cmbInventory.DisplayMemberPath = "Make";
            cmbInventory.SelectedValuePath = "CardId";
            BindDataGrid();

        }
        private void BindDataGrid()
        {
            var queryOrder = from ord in ctx.Orders
                             join cust in ctx.Customers on ord.CustId equals
                             cust.CustId
                             join inv in ctx.Iventories on ord.CardId
                             equals inv.CardId
                             select new
                             {
                                 ord.OrderId,
                                 ord.CardId,
                                 ord.CustId,
                                 cust.FirstNAme,
                                 cust.LastName,
                                 inv.Make,
                                 inv.Color
                             };
            customerOrdersViewSource.Source = queryOrder.ToList();
        }
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            Customer customer = null;
            if (action == ActionState.New)
            {
                try
                {
                    //instantiem Customer entity
                    customer = new Customer()
                    {
                        FirstNAme = firstNAmeTextBox.Text.Trim(),
                        LastName = lastNameTextBox.Text.Trim()
                    };
                    //adaugam entitatea nou creata in context
                    ctx.Customers.Add(customer);
                    customerViewSource.View.Refresh();
                    //salvam modificarile
                    ctx.SaveChanges();
                }
                //using System.Data;
                catch (DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                btnNew.IsEnabled = true;
                btnEdit.IsEnabled = true;
                btnSave.IsEnabled = false;
                btnCancel.IsEnabled = false;
                btnPrevious.IsEnabled = true;
                btnNext.IsEnabled = true;
                lastNameTextBox.IsEnabled = false;
                firstNAmeTextBox.IsEnabled = false;
            }

            else
            {
                if (action == ActionState.Edit)
                {
                    try
                    {
                        customer = (Customer)customerDataGrid.SelectedItem;
                        customer.FirstNAme = firstNAmeTextBox.Text.Trim();
                        customer.LastName = lastNameTextBox.Text.Trim();
                        //salvam modificarile
                        ctx.SaveChanges();
                    }
                    catch (DataException ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                    customerViewSource.View.Refresh();
                    // pozitionarea pe item-ul curent
                    customerViewSource.View.MoveCurrentTo(customer);
                    btnNew.IsEnabled = true;
                    btnEdit.IsEnabled = true;
                    btnDelete.IsEnabled = true;
                    btnSave.IsEnabled = false;
                    btnCancel.IsEnabled = false;
                    btnPrevious.IsEnabled = true;
                    btnNext.IsEnabled = true;
                    lastNameTextBox.IsEnabled = false;
                    firstNAmeTextBox.IsEnabled = false;
                    lastNameTextBox.SetBinding(TextBox.TextProperty, txtLastNameBinding);
                    firstNAmeTextBox.SetBinding(TextBox.TextProperty, txtFirstNameBinding);
                }

                else
                {
                    if (action == ActionState.Delete)
                    {
                        try
                        {
                            customer = (Customer)customerDataGrid.SelectedItem;
                            ctx.Customers.Remove(customer);
                            ctx.SaveChanges();
                        }
                        catch (DataException ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                        customerViewSource.View.Refresh();
                        btnNew.IsEnabled = true;
                        btnEdit.IsEnabled = true;
                        btnDelete.IsEnabled = true;
                        btnSave.IsEnabled = false;
                        btnCancel.IsEnabled = false;
                        btnPrevious.IsEnabled = true;
                        btnNext.IsEnabled = true;
                        lastNameTextBox.IsEnabled = false;
                        firstNAmeTextBox.IsEnabled = false;
                        lastNameTextBox.SetBinding(TextBox.TextProperty, txtLastNameBinding);
                        firstNAmeTextBox.SetBinding(TextBox.TextProperty, txtFirstNameBinding);
                    }
                }

            }
        }
        private void btnSave1_Click(object sender, RoutedEventArgs e)
        {
            Iventory inventory = null;
            if (action == ActionState.New)
            {
                try
                {
                    inventory = new Iventory()
                    {
                        Make = makeTextBox.Text.Trim(),
                        Color = colorTextBox.Text.Trim()
                    };

                    ctx.Iventories.Add(inventory);
                    iventoryViewSource.View.Refresh();
                    ctx.SaveChanges();
                }
                //using System.Data;
                catch (DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                btnNew1.IsEnabled = true;
                btnEdit1.IsEnabled = true;
                btnSave1.IsEnabled = false;
                btnCancel1.IsEnabled = false;
                btnPrevious1.IsEnabled = true;
                btnNext1.IsEnabled = true;
                colorTextBox.IsEnabled = false;
                makeTextBox.IsEnabled = false;
            }

            else
            {
                if (action == ActionState.Edit)
                {
                    try
                    {
                        inventory = (Iventory)iventoryDataGrid.SelectedItem;
                        inventory.Make = makeTextBox.Text.Trim();
                        inventory.Color = colorTextBox.Text.Trim();
                        ctx.SaveChanges();
                    }
                    catch (DataException ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                    iventoryViewSource.View.Refresh();
                    iventoryViewSource.View.MoveCurrentTo(inventory);
                    btnNew1.IsEnabled = true;
                    btnEdit1.IsEnabled = true;
                    btnDelete1.IsEnabled = true;
                    btnSave1.IsEnabled = false;
                    btnCancel1.IsEnabled = false;
                    btnPrevious1.IsEnabled = true;
                    btnNext1.IsEnabled = true;
                    makeTextBox.IsEnabled = false;
                    colorTextBox.IsEnabled = false;
                    makeTextBox.SetBinding(TextBox.TextProperty, txtMakeBinding);
                    colorTextBox.SetBinding(TextBox.TextProperty, txtColorBinding);
                }

                else
                {
                    if (action == ActionState.Delete)
                    {
                        try
                        {
                            inventory = (Iventory)iventoryDataGrid.SelectedItem;
                            ctx.Iventories.Remove(inventory);
                            ctx.SaveChanges();
                        }
                        catch (DataException ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                        customerViewSource.View.Refresh();
                        btnNew1.IsEnabled = true;
                        btnEdit1.IsEnabled = true;
                        btnDelete1.IsEnabled = true;
                        btnSave1.IsEnabled = false;
                        btnCancel1.IsEnabled = false;
                        btnPrevious1.IsEnabled = true;
                        btnNext1.IsEnabled = true;
                        makeTextBox.IsEnabled = false;
                        colorTextBox.IsEnabled = false;
                        makeTextBox.SetBinding(TextBox.TextProperty, txtMakeBinding);
                        colorTextBox.SetBinding(TextBox.TextProperty, txtColorBinding);
                    }
                }

            }
        }
        private void btnNext_Click(object sender, RoutedEventArgs e)
        {
            customerViewSource.View.MoveCurrentToNext();
        }
        private void btnNext1_Click(object sender, RoutedEventArgs e)
        {
            iventoryViewSource.View.MoveCurrentToNext();
        }
        private void btnPrevious_Click(object sender, RoutedEventArgs e)
        {
            customerViewSource.View.MoveCurrentToPrevious();
        }
        private void btnPrevious1_Click(object sender, RoutedEventArgs e)
        {
            iventoryViewSource.View.MoveCurrentToPrevious();
        }
        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.Edit;
            string tempFirstName = firstNAmeTextBox.Text.ToString();
            string tempLastName = lastNameTextBox.Text.ToString();
            btnNew.IsEnabled = false;
            btnEdit.IsEnabled = false;
            btnDelete.IsEnabled = false;
            btnSave.IsEnabled = true;
            btnCancel.IsEnabled = true;
            btnPrevious.IsEnabled = false;
            btnNext.IsEnabled = false;
            lastNameTextBox.IsEnabled = true;
            firstNAmeTextBox.IsEnabled = true;
            BindingOperations.ClearBinding(firstNAmeTextBox, TextBox.TextProperty);
            BindingOperations.ClearBinding(lastNameTextBox, TextBox.TextProperty);
            firstNAmeTextBox.Text = tempFirstName;
            lastNameTextBox.Text = tempLastName;
            Keyboard.Focus(firstNAmeTextBox);
            
        }
        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.Delete;
            string tempPhonenum = firstNAmeTextBox.Text.ToString();
            string tempSubscriber = lastNameTextBox.Text.ToString();
            btnNew.IsEnabled = false;
            btnEdit.IsEnabled = false;
            btnDelete.IsEnabled = false;
            btnSave.IsEnabled = true;
            btnCancel.IsEnabled = true;
            btnPrevious.IsEnabled = false;
            btnNext.IsEnabled = false;
            BindingOperations.ClearBinding(firstNAmeTextBox, TextBox.TextProperty);
            BindingOperations.ClearBinding(lastNameTextBox, TextBox.TextProperty);
            firstNAmeTextBox.Text = tempPhonenum;
            lastNameTextBox.Text = tempSubscriber;
        }
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.Nothing;
            btnNew.IsEnabled = true;
            btnEdit.IsEnabled = true;
            btnEdit.IsEnabled = true;
            btnSave.IsEnabled = false;
            btnCancel.IsEnabled = false;
            btnPrevious.IsEnabled = true;
            btnNext.IsEnabled = true;
            firstNAmeTextBox.IsEnabled = false;
            lastNameTextBox.IsEnabled = false;
            firstNAmeTextBox.SetBinding(TextBox.TextProperty, txtFirstNameBinding);
            lastNameTextBox.SetBinding(TextBox.TextProperty, txtLastNameBinding);
        }
        private void btnNew_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.New;
            btnNew.IsEnabled = false;
            btnEdit.IsEnabled = false;
            btnDelete.IsEnabled = false;
            btnSave.IsEnabled = true;
            btnCancel.IsEnabled = true;
            btnPrevious.IsEnabled = false;
            btnNext.IsEnabled = false;
            lastNameTextBox.IsEnabled = true;
            firstNAmeTextBox.IsEnabled = true;
            BindingOperations.ClearBinding(lastNameTextBox, TextBox.TextProperty);
            BindingOperations.ClearBinding(firstNAmeTextBox, TextBox.TextProperty);
            lastNameTextBox.Text = "";
            firstNAmeTextBox.Text = "";
            Keyboard.Focus(firstNAmeTextBox);
        }
        private void btnEdit1_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.Edit;
            string tempMake = makeTextBox.Text.ToString();
            string tempColor = colorTextBox.Text.ToString();
            btnNew1.IsEnabled = false;
            btnEdit1.IsEnabled = false;
            btnDelete1.IsEnabled = false;
            btnSave1.IsEnabled = true;
            btnCancel1.IsEnabled = true;
            btnPrevious1.IsEnabled = false;
            btnNext1.IsEnabled = false;
            makeTextBox.IsEnabled = true;
            colorTextBox.IsEnabled = true;
            BindingOperations.ClearBinding(colorTextBox, TextBox.TextProperty);
            BindingOperations.ClearBinding(makeTextBox, TextBox.TextProperty);
            makeTextBox.Text = tempMake;
            colorTextBox.Text = tempColor;
            Keyboard.Focus(makeTextBox);
        }
        private void btnDelete1_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.Edit;
            string tempMake = makeTextBox.Text.ToString();
            string tempColor = colorTextBox.Text.ToString();
            btnNew1.IsEnabled = false;
            btnEdit1.IsEnabled = false;
            btnDelete1.IsEnabled = false;
            btnSave1.IsEnabled = true;
            btnCancel1.IsEnabled = true;
            btnPrevious1.IsEnabled = false;
            btnNext1.IsEnabled = false;
            BindingOperations.ClearBinding(colorTextBox, TextBox.TextProperty);
            BindingOperations.ClearBinding(makeTextBox, TextBox.TextProperty);
            makeTextBox.Text = tempMake;
            colorTextBox.Text = tempColor;
        }
        private void btnCancel1_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.Nothing;
            btnNew1.IsEnabled = true;
            btnEdit1.IsEnabled = true;
            btnEdit1.IsEnabled = true;
            btnSave1.IsEnabled = false;
            btnCancel1.IsEnabled = false;
            btnPrevious1.IsEnabled = true;
            btnNext1.IsEnabled = true;
            makeTextBox.IsEnabled = false;
            colorTextBox.IsEnabled = false;
            makeTextBox.SetBinding(TextBox.TextProperty, txtMakeBinding);
            colorTextBox.SetBinding(TextBox.TextProperty, txtColorBinding);
        }
        private void btnNew1_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.New;
            btnNew1.IsEnabled = false;
            btnEdit1.IsEnabled = false;
            btnDelete1.IsEnabled = false;
            btnSave1.IsEnabled = true;
            btnCancel1.IsEnabled = true;
            btnPrevious1.IsEnabled = false;
            btnNext1.IsEnabled = false;
            makeTextBox.IsEnabled = true;
            colorTextBox.IsEnabled = true;
            BindingOperations.ClearBinding(makeTextBox, TextBox.TextProperty);
            BindingOperations.ClearBinding(colorTextBox, TextBox.TextProperty);
            makeTextBox.Text = "";
            colorTextBox.Text = "";
            Keyboard.Focus(makeTextBox);
        }
        private void btnSave0_Click(object sender, RoutedEventArgs e)
        {
            Order order = null;
            if (action == ActionState.New)
            {
                try
                {
                    Customer customer = (Customer)cmbCustomers.SelectedItem;
                    Iventory iventory = (Iventory)cmbInventory.SelectedItem;
                    //instantiem Order entity
                    order = new Order()
                    {

                        CustId = customer.CustId,
                        CardId = iventory.CardId
                    };
                    //adaugam entitatea nou creata in context
                    ctx.Orders.Add(order);
                    customerOrdersViewSource.View.Refresh();
                    //salvam modificarile
                    ctx.SaveChanges();
                }
                catch (DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                if (action == ActionState.Edit)
                {
                    dynamic selectedOrder = ordersDataGrid.SelectedItem;
                    try
                    {
                        int curr_id = selectedOrder.OrderId;
                        var editedOrder = ctx.Orders.FirstOrDefault(s => s.OrderId == curr_id);
                        if (editedOrder != null)
                        {
                            editedOrder.CustId = Int32.Parse(cmbCustomers.SelectedValue.ToString());
                            editedOrder.CardId = Convert.ToInt32(cmbInventory.SelectedValue.ToString());
                            //salvam modificarile
                            ctx.SaveChanges();
                        }
                    }
                    catch (DataException ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                    BindDataGrid();
                    // pozitionarea pe item-ul curent
                    customerViewSource.View.MoveCurrentTo(selectedOrder);
                }
                else if (action == ActionState.Delete)
                {
                    try
                    {
                        dynamic selectedOrder = ordersDataGrid.SelectedItem;
                        int curr_id = selectedOrder.OrderId;
                        var deletedOrder = ctx.Orders.FirstOrDefault(s => s.OrderId == curr_id);
                        if (deletedOrder != null)
                        {
                            ctx.Orders.Remove(deletedOrder);
                            ctx.SaveChanges();
                            MessageBox.Show("Order Deleted Successfully", "Message");
                            BindDataGrid();
                        }
                    }
                    catch (DataException ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
            }
        }
    }
}


